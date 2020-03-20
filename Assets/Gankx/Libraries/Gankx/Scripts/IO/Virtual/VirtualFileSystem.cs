using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Gankx.IO
{
    public class VirtualFileSystem : FileSystem
    {
        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DiskHeader
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string idString; // Id string of the file

            [MarshalAs(UnmanagedType.I4)]
            public int version; // Version number of file

            [MarshalAs(UnmanagedType.I4)]
            public int numFiles; // Total number of files in the archive

            [MarshalAs(UnmanagedType.I4)]
            public int numBlocks; // Total number of data blocks in the archive

            [MarshalAs(UnmanagedType.I4)]
            public int blockTableSize; // Size of the table of data block descriptions

            [MarshalAs(UnmanagedType.I4)]
            public int nameTableSize; // Size of the name table in bytes

            [MarshalAs(UnmanagedType.I8)]
            public long indexOffset; // Offset of the index within the archive file

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 504)]
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            public int[] pad; // Padding, makes the header 2048 bytes
        }

        public class Block
        {
            public long offset;
            public int compressedSize;
            public int expandedSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DiskBlock
        {
            [MarshalAs(UnmanagedType.I8)]
            public long offset; // Offset of the block within the archive file

            [MarshalAs(UnmanagedType.I4)]
            public int compressedSize; // Size of the data block in the archive file (compressed size)

            [MarshalAs(UnmanagedType.I4)]
            public int expandedSize; // Size of the data block in memory (uncompressed size)
        }

        /**
         * Buffer containing the data of a single block
         */

        public class BlockBuffer
        {
            public int index; // Index of the block
            public byte[] compressedBuffer; // Buffer containing the compressed data for the block
            public byte[] expandedBuffer; // Buffer containing the expanded buffer for the block
        }

        /**
         * Structure containing the information about a single file within the archive
         */

        public class FileEntry
        {
            public FileFlag flags; // Flags indicating the properties of the file such as compression
            public int index; // Index of the file with in the archive
            public long offset; // Offset of the start of the file with in the archive
            public int size; // Size of the file in the archive (if compressed the compressed size)
            public int expandedSize; // Actual size of the file (uncompressed)
            public int numBlocks; // Number of blocks making up the file if compressed
            public int checksum; // Checksum of the file data
            public long fileTime; // The file time
            public Block[] blockSet; // Pointer to the array of the file's block info structures
            public string name; // Pointer to the name of the file
            public FileEntry next; // Pointer to the next entry in the hash table
        }

        /**
         * File entry written to disk
         */

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DiskFileEntry
        {
            [MarshalAs(UnmanagedType.I4)]
            public int flags; // Flags indicating the properties of the file such as compression

            [MarshalAs(UnmanagedType.I8)]
            public long offset; // Offset of the start of the file with in the archive

            [MarshalAs(UnmanagedType.I4)]
            public int size; // Size of the file in the archive (if compressed the compressed size)

            [MarshalAs(UnmanagedType.I4)]
            public int expandedSize; // Actual size of the file (uncompressed)

            [MarshalAs(UnmanagedType.I4)]
            public int checksum; // Checksum of the data in the file

            [MarshalAs(UnmanagedType.I8)]
            public long fileTime; // Low order part of the file time

            [MarshalAs(UnmanagedType.I4)]
            public int numBlocks; // Number of blocks making up the file if compressed

            [MarshalAs(UnmanagedType.I4)]
            public int blockOffset; // Index of the first block in the file's block set

            [MarshalAs(UnmanagedType.I4)]
            public int nameLength; // Length of the name of the file

            [MarshalAs(UnmanagedType.I4)]
            public int nameOffset; // Offset of the name within the name table
        }

        /**
         * Archive statistics structure
         */

        public class Stats
        {
            // ReSharper disable once NotAccessedField.Global
            public int numFileEntries;
            public int numUsedFiles;

            // ReSharper disable once NotAccessedField.Global
            public int tableSize;
            public int maxDepth;
            public int indexSize;
            public int maxFileName;

            // ReSharper disable once NotAccessedField.Global
            public float averageDepth;
        }

        /**
         * Extract all of the files in the archive to the specified directory. This function will fail
         * if the specified directory is not valid.
         */

        public delegate void ProgressFunction(FileEntry fileEntry, int current, int total);
        /**
         * Modes in which an archive file may be opened
         */

        public enum FileMode
        {
            Read, // Allow only reading of files, no add or remove
            Edit, // Allow reading of files as well as adding and removing files
            Closed // Closed
        }

        /**
         * File entry flags defining file specific properties of the file entry
         */

        [Flags]
        public enum FileFlag
        {
            Used = 1, // Flag indicating if the file entry is currently used
            Compressed = 2 // Flag indicating that the file is compressed      
        }

        private const int ArchiveVersion = 1;
        private const int ArchiveGrowSize = 256;
        private const int ArchiveFileBufferSize = 4096;
        private const string ArchiveIdString = "VFS";
        private const int ArchiveDefaultBlockSize = 16 * 1024;

        // 2048 bytes
        private static readonly int SizeOfDiskHeader = Marshal.SizeOf(typeof(DiskHeader));

        // Size = 16
        private static readonly int SizeOfDiskBlock = Marshal.SizeOf(typeof(DiskBlock));

        // Size = 48
        private static readonly int SizeOfDiskFileEntry = Marshal.SizeOf(typeof(DiskFileEntry));

        private static readonly int[] ArchiveHashSizes =
        {
            31,
            73,
            127,
            283,
            419,
            739,
            1019,
            1453,
            2063,
            3001,
            4001,
            5009,
            6067,
            7001,
            8009,
            9011,
            10007
        };

        private static readonly int ArchiveNumHashSizes = ArchiveHashSizes.Length;

        private SafeStream myArchiveFileHandle; // File handle of the archive file
        private FileMode myArchiveFileMode; // Mode in which the archive is open (read or edit)
        private string myArchiveFileName; // File name (including path) of the current archive file
        private List<FileEntry> myFileArray; // Array of file entries
        private List<int> myFreeList; // List of unused file entries
        private int myBlockSize; // Size of a complete data block
        private Block[] myBlockArray; // Array of data block descriptions
        private int myTableSize; // Number of entires in the file hash table
        private FileEntry[] myFileTable; // File entry hash table

        public VirtualFileSystem()
        {
            myArchiveFileHandle = null;
            myArchiveFileMode = FileMode.Closed;
            myArchiveFileName = "";
            myBlockSize = ArchiveDefaultBlockSize;
            myBlockArray = null;
            myFileTable = null;
            myTableSize = 0;

            myFileArray = new List<FileEntry>();
            myFreeList = new List<int>();
        }

        private string GetFileModeString()
        {
            switch (myArchiveFileMode)
            {
                case FileMode.Edit:
                {
                    return "edit file(" + GetArchiveName() + ") mode";
                }

                case FileMode.Read:
                {
                    return "read file(" + GetArchiveName() + ") mode";
                }

                case FileMode.Closed:
                {
                    return "closed file mode";
                }

                default:
                {
                    return "invalid file mode";
                }
            }
        }

        private void Error(string title, string msg)
        {
            Debug.LogError(string.Format("VirtualFileSystem.{0} on {1} occurred error: {2}", title, GetFileModeString(),
                msg));
        }

        private void Warning(string title, string msg)
        {
            Debug.LogWarning(string.Format("VirtualFileSystem.{0} on {1} occurred warning: {2}", title,
                GetFileModeString(), msg));
        }

        private void Info(string title, string msg)
        {
            Debug.Log(string.Format("VirtualFileSystem.{0} on {1} info: {2}", title, GetFileModeString(), msg));
        }

        private bool Open(SafeStream archiveHandle, string fileName, FileMode mode)
        {
            myArchiveFileHandle = archiveHandle;

            // See if the file was opened successfully
            if (myArchiveFileHandle == null)
            {
                Error("Open", "Cannot open file (" + fileName + ") in mode(" + mode + ")");
                return false;
            }

            // Set the archive file mode and name
            myArchiveFileMode = mode;
            myArchiveFileName = fileName;

            // Read the archive file header if the file is not empty
            if (myArchiveFileHandle.length > 0)
            {
                if (ReadHeader() == false)
                {
                    return false;
                }
            }

            // Construct the file hash table using the file indices
            ConstructFileHashTable();

            // The archive file was successfully opened
            return true;
        }

        public bool Open(string fileName, FileMode mode)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Error("Open", "Invalid parameter: file name is empty or null");
                return false;
            }

            SafeStream archiveHandle;

            if (mode == FileMode.Read)
            {
                archiveHandle = SafeFileStream.OpenRead(fileName);
            }
            else if (mode == FileMode.Edit)
            {
                archiveHandle = SafeFileStream.OpenEdit(fileName);
            }
            else
            {
                Error("Open", "Invalid mode!");
                return false;
            }

            return Open(archiveHandle, fileName, mode);
        }

        public bool Open(byte[] fileBuffer, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Error("Open", "Invalid parameter: file name is empty or null");
                return false;
            }

            if (null == fileBuffer)
            {
                Error("Open", "Invalid parameter: file buffer is null");
                return false;
            }

            var archiveHandle = SafeMemoryStream.Open(fileBuffer);

            return Open(archiveHandle, fileName, FileMode.Read);
        }

        public override void Close()
        {
            // If the archive was open for edit the index and header must be written before the file is closed
            if (myArchiveFileMode == FileMode.Edit)
            {
                WriteHeader();

                // Delete the file names and blocks
                for (var index = 0; index < myFileArray.Count; ++index)
                {
                    var entry = myFileArray[index];

                    entry.name = null;
                    entry.blockSet = null;
                }

                myFileArray.Clear();
            }

            // Close the archive file
            if (myArchiveFileHandle != null)
            {
                myArchiveFileHandle.Close();
                myArchiveFileHandle = null;
            }

            if (true /*|| (myArchiveFileMode == FileMode.Edit)*/)
            {
                // Delete the block array
                myBlockArray = null;

                // Delete the file table
                myFileTable = null;

                // Free the file buffer
                FreeFileBuffer();

                // Erase the file name
                myArchiveFileName = "";
            }

            myArchiveFileMode = FileMode.Closed;
        }

        public void Compact()
        {
            // The compact operation may only be done while the archive is open for edit
            if (myArchiveFileMode != FileMode.Edit)
            {
                return;
            }

            // Copy the original file array
            var originalFileArray = new List<FileEntry>(myFileArray);

            // Copy the original files
            var numActiveFiles = 0;
            var bufferSize = 0;
            byte[] buffer = null;
            var currentOffset = SizeOfDiskHeader;

            for (var index = 0; index < originalFileArray.Count; ++index)
            {
                var originalEntry = originalFileArray[index];

                if ((originalEntry.flags & FileFlag.Used) != 0)
                {
                    var newEntry = new FileEntry();
                    myFileArray[numActiveFiles] = newEntry;

                    // Copy the data from the original entry
                    newEntry.flags = originalEntry.flags;
                    newEntry.index = originalEntry.index;
                    newEntry.offset = originalEntry.offset;
                    newEntry.size = originalEntry.size;
                    newEntry.expandedSize = originalEntry.expandedSize;
                    newEntry.numBlocks = originalEntry.numBlocks;
                    newEntry.checksum = originalEntry.checksum;
                    newEntry.fileTime = originalEntry.fileTime;
                    newEntry.blockSet = originalEntry.blockSet;
                    newEntry.name = originalEntry.name;
                    newEntry.next = originalEntry.next;

                    if (newEntry.offset != currentOffset)
                    {
                        // Update the file and block offsets
                        newEntry.offset = currentOffset;

                        var blockOffset = newEntry.offset;

                        for (var iBlock = 0; iBlock < newEntry.numBlocks; ++iBlock)
                        {
                            var block = newEntry.blockSet[iBlock];

                            block.offset = blockOffset;
                            blockOffset += block.compressedSize;
                        }

                        // Allocate the file buffer if needed
                        if (bufferSize < originalEntry.size)
                        {
                            // Allocate the new buffer
                            bufferSize = originalEntry.size;
                            buffer = new byte[bufferSize];
                        }

                        // Read the file from its old location
                        SetArchiveFilePointer(originalEntry.offset);
                        myArchiveFileHandle.Read(buffer, 0, originalEntry.size);

                        // Copy the file to its new location
                        SetArchiveFilePointer(newEntry.offset);
                        myArchiveFileHandle.Write(buffer, 0, newEntry.size);
                    }

                    currentOffset += newEntry.size;
                    ++numActiveFiles;
                }
            }

            // Resize the file array to hold only the active files            
            myFileArray.RemoveRange(numActiveFiles, myFileArray.Count - numActiveFiles);

            // Rebuild the hash table
            ConstructFileHashTable();
        }

        public int FindFile(string fileName)
        {
            // Check the input file name
            if (string.IsNullOrEmpty(fileName))
            {
                return -1;
            }

            // Convert the filename to the proper format
            var convertedName = ConvertFileName(fileName);

            // Calculate the hash value of the file name
            var length = convertedName.Length;
            var hashValue = HashFunction(convertedName);

            if (hashValue >= myTableSize)
            {
                Error("FindFile", "The hash value of file name(" + convertedName + ") is invalid!");
                return -1;
            }

            // Search the file hash table location for the file
            uint iteration = 0;

            var fileEntry = myFileTable[hashValue];

            while (fileEntry != null)
            {
                if (iteration >= 1000)
                {
                    Error("FindFile", "Iteration is great than 1000");
                    return -1;
                }

                if (length == fileEntry.name.Length)
                {
                    if (string.Compare(convertedName, fileEntry.name, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return fileEntry.index;
                    }
                }

                ++iteration;
                fileEntry = fileEntry.next;
            }

            // The file was not found
            return -1;
        }

        public int AddFileFromMemory(string fileName, byte[] fileData, int fileSize, int compression)
        {
            var fileIndex = -1;

            if (compression > 0)
            {
                // Calculate the number of blocks that will be required for the file.
                // Note that the (blockSize - 1) is added in order to round up the result.
                var numBlocks = (fileSize + (myBlockSize - 1)) / myBlockSize;

                // Calculate the maximum size buffer that will be needed to hold the compressed file
                // int maxCompressSize = compressBound(myBlockSize);
                var maxCompressSize = myBlockSize;

                // Allocate the compression buffer and block set
                var compressionBuffer = new byte[maxCompressSize * numBlocks];
                var blockSet = new Block[numBlocks];

                //============================================================
                // Compress the file into blocks
                //============================================================
                var readOffset = 0;
                var writeOffset = 0;
                var blockIndex = 0;

                while (readOffset < fileSize)
                {
                    // Get the block info structure
                    // assert(blockIndex < numBlocks);
                    var blockInfo = new Block();
                    blockSet[blockIndex++] = blockInfo;

                    // Determine how much data to process. The amount will be the size of a block
                    // unless the remaining data in the file is less than the size of a block.
                    var bytesToWrite = Math.Min(fileSize - readOffset, myBlockSize);

                    // Compress the block data
                    var compressedSize = maxCompressSize;
                    Buffer.BlockCopy(fileData, readOffset, compressionBuffer, writeOffset, bytesToWrite);

                    blockInfo.offset = writeOffset;
                    blockInfo.compressedSize = compressedSize;
                    blockInfo.expandedSize = bytesToWrite;

                    readOffset += bytesToWrite;
                    writeOffset += compressedSize;
                }

                // Add the file entry into the archive table. The writeOffset
                // is the total size of the compressed file.
                fileIndex = AddFileEntry(fileName, writeOffset, fileSize, true);

                Debug.Log("Add file entry on memory:" + fileIndex);

                //============================================================
                // Write the data to the specified location in the archive
                //============================================================
                if (fileIndex >= 0)
                {
                    var entry = myFileArray[fileIndex];

                    // Move to the file location with the archive and write the data
                    SetArchiveFilePointer(entry.offset);
                    myArchiveFileHandle.Write(compressionBuffer, 0, writeOffset);

                    // Update the file entry
                    entry.numBlocks = numBlocks;
                    entry.blockSet = blockSet;

                    // Update the block offsets to include the offset of the the file
                    for (var iBlock = 0; iBlock < numBlocks; ++iBlock)
                    {
                        blockSet[iBlock].offset += entry.offset;
                    }
                }
            }
            else
            {
                // Add the entry
                fileIndex = AddFileEntry(fileName, fileSize, fileSize, false);

                // Write the file data to the specified location in the archive
                if (fileIndex >= 0)
                {
                    SetArchiveFilePointer(myFileArray[fileIndex].offset);
                    myArchiveFileHandle.Write(fileData, 0, fileSize);
                }
            }

            // Return the index of the file that was added
            return fileIndex;
        }

        public int AddFileFromDisk(string fileName, int compression)
        {
            int fileIndex;

            // Check the provided file name
            if (string.IsNullOrEmpty(fileName))
            {
                Error("AddFileFromDisk", "Invalid parameter: filename is empty or null!");
                return -1;
            }

            if (compression > 0)
            {
                fileIndex = AddFileFromDiskCompressed(fileName, compression);

                Debug.Log("Add file from disk compressed:" + fileIndex);

                if (fileIndex < 0)
                {
                    fileIndex = AddFileFromDiskUncompressed(fileName);
                }
            }
            else
            {
                fileIndex = AddFileFromDiskUncompressed(fileName);
            }

            // Update the file time of the entry
            if (fileIndex >= 0)
            {
                var fileEntry = myFileArray[fileIndex];
                fileEntry.fileTime = 0;
            }

            return fileIndex;
        }

        public void FreeFileBuffer()
        {
        }

        public bool ExtractFileToMemory(int fileIndex, byte[] dst, int maxBytes)
        {
            var entry = myFileArray[fileIndex];

            ReadFromFile(fileIndex, dst, 0, 0, Math.Min(maxBytes, entry.expandedSize));

            return true;
        }

        public bool ExtractFileToDisk(int fileIndex, string directory)
        {
            // Make sure that the index is valid
            if (fileIndex < 0 || fileIndex >= myFileArray.Count)
            {
                Warning("ExtractFileToDisk",
                    "The fileIndex(" + fileIndex + ") is out of range[0," + myFileArray.Count + ")!");
                return false;
            }

            // Get the file entry of the file to be extracted
            var entry = myFileArray[fileIndex];

            Debug.Log("ExtractFileToDisk:" + entry.flags);

            // Make sure the file is actually valid
            if ((entry.flags & FileFlag.Used) == 0)
            {
                return false;
            }

            // Construct the name of the output file
            var outputFileName = ConvertFileName(directory);
            if (outputFileName.Last() != '/')
            {
                outputFileName += '/';
            }

            outputFileName += entry.name;

            // Create the directories needed for the file
            var path = outputFileName;
            var pathEnd = path.LastIndexOf('/');

            if (pathEnd > 0)
            {
                path = path.Remove(pathEnd);
                CreatePath(path);
            }

            // Open the output file
            var outputFile = SafeFileStream.OpenWrite(outputFileName);
            if (outputFile == null)
            {
                Error("ExtractFileToDisk", "Cannot open the output file(" + outputFileName + ")");
                return false;
            }

            // Write the file data to disk, decompressing first if needed
            if ((entry.flags & FileFlag.Compressed) != 0)
            {
                ExtractFileToDiskCompressed(fileIndex, entry, outputFile);
            }
            else
            {
                ExtractFileToDiskUncompressed(entry, outputFile);
            }

            // Close the output file
            outputFile.Close();

            return true;
        }

        public bool ExtractAll(string directory, ProgressFunction progressFunc)
        {
            var success = true;

            // Extract all of the files to disk
            for (var iFile = 0; iFile < myFileArray.Count; ++iFile)
            {
                // Update the progress function if specified
                if (progressFunc != null)
                {
                    progressFunc(myFileArray[iFile], iFile, myFileArray.Count);
                }

                // Extract the file
                if (ExtractFileToDisk(iFile, directory) == false)
                {
                    success = false;
                }
            }

            return success;
        }

        public bool ReadFromFile(int fileIndex, byte[] dest, int destOffset, int offset, int size,
            BlockBuffer blockBuffer = null)
        {
            // Check the output buffer
            if (dest == null)
            {
                Warning("ReadFromFile", "The dest buffer is null!");
                return false;
            }

            // Check the validity of the file index
            if (fileIndex < 0 || fileIndex >= myFileArray.Count)
            {
                Warning("ReadFromFile",
                    "The fileIndex(" + fileIndex + ") is out of range[0," + myFileArray.Count + ")!");
                return false;
            }

            // Get the file entry
            var entry = myFileArray[fileIndex];

            // Check the offset and size
            if (offset > entry.expandedSize)
            {
                return false;
            }

            if (offset + size > entry.expandedSize)
            {
                size = entry.size - offset;
            }

            if ((entry.flags & FileFlag.Compressed) != 0)
            {
                if (ReadCompressedData(fileIndex, dest, destOffset, offset, size, blockBuffer) == false)
                {
                    return false;
                }
            }
            else
            {
                SetArchiveFilePointer(entry.offset + offset);
                myArchiveFileHandle.Read(dest, destOffset, size);

                VirtualEncryptHelper.LeftShiftByte(dest, destOffset, size, 1);
                VirtualEncryptHelper.XorByte(dest, destOffset, size, 0x44);
                VirtualEncryptHelper.RightShiftByte(dest, destOffset, size, 4);
            }

            return true;
        }

        public bool RemoveFile(int fileIndex)
        {
            // Must be in edit mode to remove a file
            if (myArchiveFileMode != FileMode.Edit)
            {
                Warning("RemoveFile", "Not in file edit mode!");
                return false;
            }

            // Make sure the index is valid
            if (fileIndex < 0 || fileIndex >= myFileArray.Count)
            {
                Warning("RemoveFile",
                    "The fileIndex(" + fileIndex + ") is out of range[0," + myFileArray.Count + ")!");
                return false;
            }

            // Get the file entry
            var fileEntry = myFileArray[fileIndex];

            // The file has been removed
            if ((fileEntry.flags & FileFlag.Used) == 0)
            {
                Warning("RemoveFile", "The file(" + fileEntry.name + ") at index(" + fileIndex + ") has been removed!");
                return true;
            }

            // Remove the entry from the hash table
            var hashValue = HashFunction(fileEntry.name);
            var pTableEntry = myFileTable[hashValue];

            if (pTableEntry == fileEntry)
            {
                myFileTable[hashValue] = fileEntry.next;
            }
            else
            {
                while (pTableEntry != null)
                {
                    if (pTableEntry.next == fileEntry)
                    {
                        pTableEntry.next = fileEntry.next;
                        break;
                    }

                    pTableEntry = pTableEntry.next;
                }
            }

            // Delete the file name of the entry
            fileEntry.name = null;

            // Clear the entry except the size and offset
            fileEntry.flags = 0;
            fileEntry.expandedSize = 0;
            fileEntry.checksum = 0;
            fileEntry.fileTime = 0;
            fileEntry.numBlocks = 0;
            fileEntry.blockSet = null;
            fileEntry.name = null;
            fileEntry.next = null;

            // Add the entry to the free list
            myFreeList.Add(fileIndex);

            return false;
        }

        public void CalcStats(Stats archiveStats)
        {
            // Clear the stats structure
            archiveStats.numFileEntries = myFileArray.Count;
            archiveStats.numUsedFiles = 0;
            archiveStats.tableSize = myTableSize;
            archiveStats.maxDepth = 0;
            archiveStats.indexSize = 0;
            archiveStats.maxFileName = 0;
            archiveStats.averageDepth = 0f;

            // Calculate the file entry stats
            for (var iFile = 0; iFile < myFileArray.Count; ++iFile)
            {
                var entry = myFileArray[iFile];

                if ((entry.flags & FileFlag.Used) != 0)
                {
                    ++archiveStats.numUsedFiles;
                }

                if (entry.name.Length > archiveStats.maxFileName)
                {
                    archiveStats.maxFileName = entry.name.Length;
                }

                archiveStats.indexSize += entry.name.Length + SizeOfDiskFileEntry +
                                          SizeOfDiskBlock * entry.numBlocks;
            }

            // Calculate the hash table stats
            var usedElements = 0;

            for (var iElement = 0; iElement < myTableSize; ++iElement)
            {
                var depth = 0;

                var pEntry = myFileTable[iElement];

                while (pEntry != null)
                {
                    ++depth;
                    pEntry = pEntry.next;
                }

                if (depth > 0)
                {
                    ++usedElements;
                }

                if (depth > archiveStats.maxDepth)
                {
                    archiveStats.maxDepth = depth;
                }
            }

            // make useedElements not zero [menghe]
            if (usedElements == 0)
            {
                usedElements++;
            }

            archiveStats.averageDepth = (float) archiveStats.numUsedFiles / usedElements;
        }

        public int GetNumFiles()
        {
            return myFileArray.Count;
        }

        public FileEntry GetFileEntry(int fileIndex)
        {
            return myFileArray[fileIndex];
        }

        public FileMode ArchiveFileMode()
        {
            return myArchiveFileMode;
        }

        public int BlockSize()
        {
            return myBlockSize;
        }

        public string GetArchiveName()
        {
            return myArchiveFileName;
        }

        private int AddFileEntry(string fileName, int fileSize, int expandedSize, bool compressed)
        {
            // Make sure that a file with the same name does not already exist within the archive
            if (FindFile(fileName) >= 0)
            {
                Warning("AddFileEntry", "The file(" + fileName + ") has been added!");
                return -1;
            }

            // Convert the file name to the proper format
            var convertedName = ConvertFileName(fileName);

            // Initialize the new file entry
            var newEntry = new FileEntry();

            newEntry.flags = FileFlag.Used;
            newEntry.size = fileSize;
            newEntry.expandedSize = expandedSize;
            newEntry.checksum = 0;
            newEntry.fileTime = 0;
            newEntry.next = null;
            newEntry.numBlocks = 0;
            newEntry.blockSet = null;
            newEntry.name = convertedName;

            if (compressed)
            {
                newEntry.flags |= FileFlag.Compressed;
            }

            // Get the hash value for the new entry
            var hashValue = HashFunction(newEntry.name);

            if (hashValue >= myTableSize)
            {
                // assert(hashValue < myTableSize);
                return -1;
            }

            // Find the smallest un-used file that can hold the file entry, if any.
            var fileIndex = FindSmallestFit(fileSize);

            if (fileIndex >= 0)
            {
                // Copy the offset from the file entry that the new entry is to
                // replace and replace the file entry with the new entry.
                var fileEntry = myFileArray[fileIndex];
                newEntry.offset = fileEntry.offset;
                newEntry.index = fileEntry.index;
                myFileArray[fileIndex] = newEntry;

                // Add the entry to the hash table
                newEntry.next = myFileTable[hashValue];
                myFileTable[hashValue] = newEntry;
            }
            else
            {
                // Calculate the offset of the file entry, if it is the
                // first file then it will directly follow the header.
                if (myFileArray.Count > 0)
                {
                    var lastFile = myFileArray.Last();
                    newEntry.offset = lastFile.offset + lastFile.size;
                }
                else
                {
                    newEntry.offset = SizeOfDiskHeader;
                }

                // Allocate additional elements if the array is full
                if (myFileArray.Capacity <= myFileArray.Count)
                {
                    myFileArray.Capacity = myFileArray.Count + ArchiveGrowSize;
                }

                // Add the new entry to the end of the array
                fileIndex = myFileArray.Count;
                newEntry.index = fileIndex;
                myFileArray.Add(newEntry);

                // Add the entry to the hash table
                newEntry.next = myFileTable[hashValue];
                myFileTable[hashValue] = newEntry;
            }

            // Remove the file from the free list it is in it
            myFreeList.Remove(fileIndex);

            // Return the index of the entry
            return fileIndex;
        }

        private int AddFileFromDiskUncompressed(string fileName)
        {
            // Make sure that a file with the same name does not already exist within the archive
            if (FindFile(fileName) >= 0)
            {
                Warning("AddFileFromDiskUncompressed", "The file(" + fileName + ") has been added!");
                return -1;
            }

            // Open the input file
            var inputFile = SafeFileStream.OpenRead(fileName);
            if (null == inputFile)
            {
                Error("AddFileFromDiskUncompressed", "Cannot open the input file(" + fileName + ")!");
                return -1;
            }

            // Determine the size of the file
            var fileSize = (int) inputFile.length;
            if (fileSize < 0)
            {
                Error("AddFileFromDiskUncompressed",
                    "The length(" + inputFile.length + ") of input file(" + fileName + ") is out of range[0, " +
                    int.MaxValue + ")!");
                return -1;
            }

            // Add the new file entry to the archive
            var fileIndex = AddFileEntry(fileName, fileSize, fileSize, false);

            // Copy the file data into the archive
            if (fileIndex >= 0)
            {
                // Allocate the buffer used to copy the file
                var fileBuffer = new byte[fileSize];

                // Move the location of archive file pointer to the start location of the new file entry
                SetArchiveFilePointer(myFileArray[fileIndex].offset);

                {
                    // Read the data from the source file
                    inputFile.Read(fileBuffer, 0, fileSize);

                    // Encrypt the index data	<<4 ^0x44 >>1
                    VirtualEncryptHelper.LeftShiftByte(fileBuffer, 0, fileSize, 4);
                    VirtualEncryptHelper.XorByte(fileBuffer, 0, fileSize, 0x44);
                    VirtualEncryptHelper.RightShiftByte(fileBuffer, 0, fileSize, 1);

                    // Write the data to the archive file
                    myArchiveFileHandle.Write(fileBuffer, 0, fileSize);
                }
            }

            // Close the input file
            inputFile.Close();

            return fileIndex;
        }

        // ReSharper disable once UnusedParameter.Local
        private int AddFileFromDiskCompressed(string fileName, int compression)
        {
            var fileIndex = -1;

            // Make sure that a file with the same name does not already exist within the archive
            if (FindFile(fileName) >= 0)
            {
                Warning("AddFileFromDiskCompressed", "The file(" + fileName + ") has been added!");
                return -1;
            }

            // Open the input file
            var inputFile = SafeFileStream.OpenRead(fileName);
            if (null == inputFile)
            {
                Error("AddFileFromDiskCompressed", "Cannot open the file(" + fileName + ")!");
                return -1;
            }

            // Determine the size of the file
            var fileSize = (int) inputFile.length;
            if (fileSize < 0)
            {
                Error("AddFileFromDiskCompressed",
                    "The length(" + inputFile.length + ") of input file(" + fileName + ") is out of range[0, " +
                    int.MaxValue + ")!");
                return -1;
            }

            // Calculate the number of blocks that will be required for the file.
            // Note that the (blockSize - 1) is added in order to round up the result.
            var numBlocks = (fileSize + (myBlockSize - 1)) / myBlockSize;

            // Calculate the maximum size buffer that will be needed to hold the compressed file.
            var maxCompressSize = myBlockSize;

            // Allocate the file data buffer, the compression buffer and block set.
            var fileDataBuffer = new byte[myBlockSize];
            var compressionBuffer = new byte[maxCompressSize];
            var compressedBlockData = new byte[numBlocks][];
            var blockSet = new Block[numBlocks];

            //=========================================================================
            // Compress the file into blocks
            //=========================================================================
            var blockIndex = 0;
            var fileCompressedSize = 0;

            while (blockIndex < numBlocks)
            {
                // Read the data from the file.
                var bytesToProcess = inputFile.Read(fileDataBuffer, 0, myBlockSize);

                // Stop if the end of the file has been reached or there is a file read error.
                if (bytesToProcess == 0)
                {
                    break;
                }

                // Compress the block data.
                var blockCompressedSize = maxCompressSize;

                Buffer.BlockCopy(fileDataBuffer, 0, compressionBuffer, 0, maxCompressSize);

                if (blockCompressedSize > myBlockSize)
                {
                    break;
                }

                // Allocate the buffer to hold the compressed data of the block and copy
                // the data from the temporary compression buffer.
                compressedBlockData[blockIndex] = new byte[blockCompressedSize];

                Buffer.BlockCopy(compressionBuffer, 0, compressedBlockData[blockIndex], 0, blockCompressedSize);

                // Initialize the block information structure.
                var blockInfo = new Block();
                blockSet[blockIndex] = blockInfo;
                blockInfo.offset = fileCompressedSize;
                blockInfo.compressedSize = blockCompressedSize;
                blockInfo.expandedSize = bytesToProcess;

                // Update the total size of the compressed file.
                fileCompressedSize += blockCompressedSize;

                ++blockIndex;
            }

            inputFile.Close();


            //=========================================================================
            // Add the file entry to the archive and write the compressed file data
            // into the archive file.
            //=========================================================================
            if (blockIndex == numBlocks)
            {
                // Add the file entry into the archive table. The writeOffset
                // is the total size of the compressed file.
                fileIndex = AddFileEntry(fileName, fileCompressedSize, fileSize, true);

                Debug.Log("Add file entry on compressed:" + fileIndex);

                if (fileIndex >= 0)
                {
                    var entry = myFileArray[fileIndex];

                    // Update the file entry
                    entry.numBlocks = numBlocks;
                    entry.blockSet = blockSet;
                    // entry.checksum = checksum;

                    // Move to the file location with the archive and write the data.
                    SetArchiveFilePointer(entry.offset);

                    for (var iBlock = 0; iBlock < numBlocks; ++iBlock)
                    {
                        // Write the block to the archive file.
                        myArchiveFileHandle.Write(compressedBlockData[iBlock], 0,
                            blockSet[iBlock].compressedSize);

                        // Update the block offsets to include the offset of the the file
                        blockSet[iBlock].offset += entry.offset;
                    }
                }
            }

            // Delete the compressed block data.
            for (var iBlock = 0; iBlock < numBlocks; ++iBlock)
            {
                compressedBlockData[iBlock] = null;
            }

            return fileIndex;
        }

        private void ExtractFileToDiskUncompressed(FileEntry entry, SafeStream outputFile)
        {
            // Allocate the buffer used to copy the file
            var fileBuffer = new byte[ArchiveFileBufferSize];

            // Move the location of archive file pointer to the start location of the new file entry
            SetArchiveFilePointer(entry.offset);

            // Read the file data in chunks and copy to the output file
            var bytesRemaining = entry.size;

            while (bytesRemaining > 0)
            {
                // Read the data from the source file
                var bytesToRead = Math.Min(bytesRemaining, ArchiveFileBufferSize);
                myArchiveFileHandle.Read(fileBuffer, 0, bytesToRead);
                bytesRemaining -= bytesToRead;

                // Write the data to the archive file
                outputFile.Write(fileBuffer, 0, bytesToRead);
            }
        }

        private void ExtractFileToDiskCompressed(int fileIndex, FileEntry fileEntry, SafeStream outputFile)
        {
            // Allocate a buffer to hold the decompressed output
            var outputBuffer = new byte[fileEntry.expandedSize];

            // Extract the file to the output buffer
            if (ExtractFileToMemory(fileIndex, outputBuffer, fileEntry.expandedSize))
            {
                // Write the file data to disk
                outputFile.Write(outputBuffer, 0, fileEntry.expandedSize);
            }
        }

        private bool ReadHeader()
        {
            // Make sure the file is open
            if (myArchiveFileHandle == null)
            {
                Error("ReadHeader", "The file is closed!");
                return false;
            }

            // Read the header from the archive file
            var archiveHeaderBytes = new byte[SizeOfDiskHeader];
            var bytesRead = myArchiveFileHandle.Read(archiveHeaderBytes, 0, archiveHeaderBytes.Length);

            if (bytesRead != SizeOfDiskHeader)
            {
                Error("ReadHeader",
                    "The read size(" + bytesRead + ") of header is not equal the struct size(" + SizeOfDiskHeader +
                    ") of header!");
                return false;
            }

            VirtualEncryptHelper.LeftShiftByte(archiveHeaderBytes, 0, archiveHeaderBytes.Length, 1);
            VirtualEncryptHelper.XorByte(archiveHeaderBytes, 0, archiveHeaderBytes.Length, 0x44);
            VirtualEncryptHelper.RightShiftByte(archiveHeaderBytes, 0, archiveHeaderBytes.Length, 4);

            var archiveHeaderBytesHandle = GCHandle.Alloc(archiveHeaderBytes, GCHandleType.Pinned);
            var archiveHeaderBytesHandlePtr = archiveHeaderBytesHandle.AddrOfPinnedObject();

            var diskHeader =
                (DiskHeader) Marshal.PtrToStructure(archiveHeaderBytesHandlePtr, typeof(DiskHeader));

            archiveHeaderBytesHandle.Free();

            // Verify the file id string and version number
            if (diskHeader.idString != ArchiveIdString)
            {
                Error("ReadHeader",
                    "The read idString(" + diskHeader.idString + ") of header is not equal the ARCHIVE_ID_STRING(" +
                    ArchiveIdString +
                    ")!");
                return false;
            }

            if (diskHeader.version != ArchiveVersion)
            {
                Error("ReadHeader",
                    "The read version(" + diskHeader.version + ") of header is not equal the ARCHIVE_VERSION(" +
                    ArchiveVersion +
                    ")!");
                return false;
            }

            // Read the archive file index
            return ReadIndex(ref diskHeader);
        }

        private bool ReadIndex(ref DiskHeader diskHeader)
        {
            // Move the file pointer to the start of the index
            SetArchiveFilePointer(diskHeader.indexOffset);

            // Allocate and read the block table
            if (diskHeader.numBlocks > 0)
            {
                myBlockArray = new Block[diskHeader.numBlocks];

                var diskBlockBytes = new byte[SizeOfDiskBlock];
                var diskBlockBytesHandle = GCHandle.Alloc(diskBlockBytes, GCHandleType.Pinned);
                var diskBlockBytesHandleAddr = diskBlockBytesHandle.AddrOfPinnedObject();

                for (var iIndex = 0; iIndex < myBlockArray.Length; ++iIndex)
                {
                    myArchiveFileHandle.Read(diskBlockBytes, 0, diskBlockBytes.Length);

                    var diskBlock =
                        (DiskBlock) Marshal.PtrToStructure(diskBlockBytesHandleAddr, typeof(DiskBlock));

                    var block = new Block();

                    block.offset = diskBlock.offset;
                    block.compressedSize = diskBlock.compressedSize;
                    block.expandedSize = diskBlock.expandedSize;

                    myBlockArray[iIndex] = block;
                }

                diskBlockBytesHandle.Free();
            }

            byte[] nameTableBytes = null;

            // Allocate and read the name table
            if (diskHeader.nameTableSize > 0)
            {
                nameTableBytes = new byte[diskHeader.nameTableSize];
                myArchiveFileHandle.Read(nameTableBytes, 0, nameTableBytes.Length);

                VirtualEncryptHelper.LeftShiftByte(nameTableBytes, 0, nameTableBytes.Length, 2);
                VirtualEncryptHelper.XorByte(nameTableBytes, 0, nameTableBytes.Length, 0x33);
                VirtualEncryptHelper.RightShiftByte(nameTableBytes, 0, nameTableBytes.Length, 3);
            }

            // Allocate and read the file array
            myFileArray.Clear();
            myFileArray.Capacity = diskHeader.numFiles;
            myFreeList.Clear();

            var diskFileEntryBytes = new byte[SizeOfDiskFileEntry * diskHeader.numFiles];

            // 读取文件信息。
            myArchiveFileHandle.Read(diskFileEntryBytes, 0, diskFileEntryBytes.Length);

            // Encrypt the index data	<<2 ^0x33 >>3
            VirtualEncryptHelper.LeftShiftByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 2);
            VirtualEncryptHelper.XorByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 0x33);
            VirtualEncryptHelper.RightShiftByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 3);

            var sb = new StringBuilder();
            for (var iIndex = 0; iIndex < diskFileEntryBytes.Length; ++iIndex)
            {
                sb.Append(diskFileEntryBytes[iIndex]);
            }

            var diskFileEntryBytesHandle = GCHandle.Alloc(diskFileEntryBytes, GCHandleType.Pinned);
            var diskFileEntryBytesHandleAddr = diskFileEntryBytesHandle.AddrOfPinnedObject();

            for (var iFile = 0; iFile < diskHeader.numFiles; ++iFile)
            {
                var diskFileEntry = (DiskFileEntry) Marshal.PtrToStructure(
                    new IntPtr(diskFileEntryBytesHandleAddr.ToInt64() + iFile * SizeOfDiskFileEntry),
                    typeof(DiskFileEntry));

                var fileEntry = new FileEntry();
                fileEntry.index = myFileArray.Count;
                myFileArray.Add(fileEntry);

                fileEntry.flags = (FileFlag) diskFileEntry.flags;
                fileEntry.offset = diskFileEntry.offset;
                fileEntry.size = diskFileEntry.size;
                fileEntry.expandedSize = diskFileEntry.expandedSize;
                fileEntry.checksum = diskFileEntry.checksum;
                fileEntry.fileTime = diskFileEntry.fileTime;
                fileEntry.numBlocks = diskFileEntry.numBlocks;
                fileEntry.next = null;
                fileEntry.name = null;
                fileEntry.blockSet = null;

                if ((fileEntry.flags & FileFlag.Used) != 0)
                {
                    // If the archive is open for edit each entry owns its name so that new entries can be added,
                    // but if the archive is open read only then the names can be left in the table for efficiency.
                    if (nameTableBytes != null && diskFileEntry.nameLength > 0)
                    {
                        fileEntry.name = Encoding.UTF8.GetString(nameTableBytes, diskFileEntry.nameOffset,
                            diskFileEntry.nameLength);
                    }
                    else
                    {
                        fileEntry.name = "";
                    }

                    fileEntry.blockSet = new Block[fileEntry.numBlocks];

                    if (myBlockArray != null)
                    {
                        Array.Copy(myBlockArray, diskFileEntry.blockOffset, fileEntry.blockSet, 0, fileEntry.numBlocks);
                    }
                }
                else
                {
                    // The free list is only maintained in edit mode
                    if (myArchiveFileMode == FileMode.Edit)
                    {
                        myFreeList.Add(iFile);
                    }
                }
            }

            diskFileEntryBytesHandle.Free();

            return true;
        }

        private void WriteHeader()
        {
            // Make sure the file can be modified
            if (myArchiveFileMode != FileMode.Edit)
            {
                Warning("WriteHeader", "Not in the file edit mode!");
                return;
            }

            // Initialize the header
            var diskHeader = new DiskHeader();
            diskHeader.numBlocks = 0;
            diskHeader.blockTableSize = 0;
            diskHeader.nameTableSize = 0;
            diskHeader.version = ArchiveVersion;
            diskHeader.numFiles = myFileArray.Count;
            diskHeader.idString = ArchiveIdString;

            if (myFileArray.Count > 0)
            {
                var lastFile = myFileArray.Last();
                diskHeader.indexOffset = lastFile.offset + lastFile.size;
            }
            else
            {
                diskHeader.indexOffset = SizeOfDiskHeader;
            }

            // Write the index, updating the header
            WriteIndex(ref diskHeader);

            var diskHeaderBytes = new byte[SizeOfDiskHeader];
            var diskHeaderBytesHandle = GCHandle.Alloc(diskHeaderBytes, GCHandleType.Pinned);
            var diskHeaderBytesHandleAddr = diskHeaderBytesHandle.AddrOfPinnedObject();

            Marshal.StructureToPtr(diskHeader, diskHeaderBytesHandleAddr, false);

            diskHeaderBytesHandle.Free();

            // Encrypt the index data	<<4 ^0x44 >>1
            VirtualEncryptHelper.LeftShiftByte(diskHeaderBytes, 0, SizeOfDiskHeader, 4);
            VirtualEncryptHelper.XorByte(diskHeaderBytes, 0, SizeOfDiskHeader, 0x44);
            VirtualEncryptHelper.RightShiftByte(diskHeaderBytes, 0, SizeOfDiskHeader, 1);

            // Write the header at the beginning of the file
            SetArchiveFilePointer(0);

            myArchiveFileHandle.Write(diskHeaderBytes, 0, SizeOfDiskHeader);
        }

        private void WriteIndex(ref DiskHeader diskHeader)
        {
            // Move to the location in the file where the index is to be written
            SetArchiveFilePointer(diskHeader.indexOffset);

            var diskBlock = new DiskBlock();
            var diskBlockBytes = new byte[SizeOfDiskBlock];
            var diskBlockBytesHandle = GCHandle.Alloc(diskBlockBytes, GCHandleType.Pinned);
            var diskBlockBytesHandleAddr = diskBlockBytesHandle.AddrOfPinnedObject();

            // Write the block data
            for (var iFile = 0; iFile < diskHeader.numFiles; ++iFile)
            {
                var entry = myFileArray[iFile];

                if (entry.blockSet != null && entry.blockSet.Length > 0)
                {
                    for (var iIndex = 0; iIndex < entry.numBlocks; ++iIndex)
                    {
                        diskBlock.compressedSize = entry.blockSet[iIndex].compressedSize;
                        diskBlock.expandedSize = entry.blockSet[iIndex].expandedSize;
                        diskBlock.offset = entry.blockSet[iIndex].offset;

                        Marshal.StructureToPtr(diskBlock, diskBlockBytesHandleAddr, true);

                        myArchiveFileHandle.Write(diskBlockBytes, 0, diskBlockBytes.Length);
                        diskHeader.blockTableSize += diskBlockBytes.Length;
                    }
                }
            }

            diskBlockBytesHandle.Free();

            var diskFileEntryBytes = new byte[diskHeader.numFiles * SizeOfDiskFileEntry];
            var diskFileEntry = new DiskFileEntry();

            var diskFileEntryBytesHandle = GCHandle.Alloc(diskFileEntryBytes, GCHandleType.Pinned);
            var diskFileEntryBytesHandleAddr = diskFileEntryBytesHandle.AddrOfPinnedObject();

            var fileNameList = new List<string>();

            // Write the file names and initialize the disk file entries
            for (var iFile = 0; iFile < diskHeader.numFiles; ++iFile)
            {
                var entry = myFileArray[iFile];

                diskFileEntry.flags = (int) entry.flags;
                diskFileEntry.offset = entry.offset;
                diskFileEntry.size = entry.size;
                diskFileEntry.expandedSize = entry.expandedSize;
                diskFileEntry.checksum = entry.checksum;
                diskFileEntry.fileTime = entry.fileTime;
                diskFileEntry.nameLength = 0;
                diskFileEntry.nameOffset = diskHeader.nameTableSize;
                diskFileEntry.numBlocks = entry.numBlocks;
                diskFileEntry.blockOffset = diskHeader.numBlocks;

                if (!string.IsNullOrEmpty(entry.name))
                {
                    fileNameList.Add(entry.name);

                    var nameBytes = Encoding.UTF8.GetBytes(entry.name);

                    //myArchiveFileHandle.Write(nameBytes, 0, nameBytes.Length);

                    diskFileEntry.nameLength = nameBytes.Length;
                    diskHeader.nameTableSize += nameBytes.Length;
                }


                diskHeader.numBlocks += entry.numBlocks;

                Marshal.StructureToPtr(diskFileEntry,
                    new IntPtr(diskFileEntryBytesHandleAddr.ToInt64() + SizeOfDiskFileEntry * iFile), true);
            }

            diskFileEntryBytesHandle.Free();

            //写入名字信息
            var allNamesBytes = new byte[diskHeader.nameTableSize];
            var tempNameOffset = 0;
            for (var i = 0; i < fileNameList.Count; i++)
            {
                var nameBytes = Encoding.UTF8.GetBytes(fileNameList[i]);
                Buffer.BlockCopy(nameBytes, 0, allNamesBytes, tempNameOffset, nameBytes.Length);
                tempNameOffset += nameBytes.Length;
            }

            VirtualEncryptHelper.LeftShiftByte(allNamesBytes, 0, allNamesBytes.Length, 3);
            VirtualEncryptHelper.XorByte(allNamesBytes, 0, allNamesBytes.Length, 0x33);
            VirtualEncryptHelper.RightShiftByte(allNamesBytes, 0, allNamesBytes.Length, 2);

            myArchiveFileHandle.Write(allNamesBytes, 0, allNamesBytes.Length);

            var sb = new StringBuilder();
            for (var iIndex = 0; iIndex < diskFileEntryBytes.Length; ++iIndex)
            {
                sb.Append(diskFileEntryBytes[iIndex]);
            }

            // Encrypt the index data	<<3 ^0x33 >>2
            VirtualEncryptHelper.LeftShiftByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 3);
            VirtualEncryptHelper.XorByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 0x33);
            VirtualEncryptHelper.RightShiftByte(diskFileEntryBytes, 0, diskFileEntryBytes.Length, 2);

            // Write the file entry array to disk
            myArchiveFileHandle.Write(diskFileEntryBytes, 0, diskFileEntryBytes.Length);

            // Set the end of file marker, this was if the file was
            // larger before its size will be properly reduced.
            myArchiveFileHandle.SetEndOfStream();
        }

        private int HashFunction(string fileName)
        {
            var length = fileName.Length;

            if (length < 6)
            {
                return (int) ((length + (uint) (fileName[0] << 8) + (uint) (fileName[length - 1] << 16)) % myTableSize);
            }

            return (int) ((length + (uint) (fileName[0] << 4)
                                  + (uint) (fileName[length >> 1] << 8)
                                  + (uint) (fileName[length - 1 - (length >> 3)] << 12)
                                  + (uint) (fileName[length - 1] << 16)
                                  + (uint) (fileName[length - 5] << 20)
                                  + (uint) (fileName[length - 6] << 24)) % myTableSize);
        }

        public string ConvertFileName(string fileName)
        {
            return PathUtil.NormalizePath(fileName);
        }

        private void ConstructFileHashTable()
        {
            var numFiles = myFileArray.Count;

            // Determine the size of the hash table based on the total number of files
            var iSize = 0;

            do
            {
                myTableSize = ArchiveHashSizes[iSize++];
            } while (iSize < ArchiveNumHashSizes && numFiles / 4 > myTableSize);

            // Allocate the hash table
            myFileTable = new FileEntry[myTableSize];

            // Add each of the file entires to the hash table
            for (var iFile = 0; iFile < numFiles; ++iFile)
            {
                var entry = myFileArray[iFile];

                if ((entry.flags & FileFlag.Used) != 0)
                {
                    var hashValue = HashFunction(entry.name);

                    entry.next = myFileTable[hashValue];
                    myFileTable[hashValue] = entry;
                }
            }
        }

        private void CreatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Error("CreatePath", "Invalid parameter: the path is empty or null!");
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                Error("CreatePath", "Create the path(" + path + ") occured exception: " + e);
                // ignored
            }
        }

        private int FindSmallestFit(long size)
        {
            var fileIndex = -1;
            var smallest = long.MaxValue;

            for (var index = 0; index < myFreeList.Count; ++index)
            {
                var file = myFileArray[myFreeList[index]];

                if (file.size >= size)
                {
                    // If the entry is equal there can't be any better
                    // fit so there is no need to continue the search.
                    if (file.size == size)
                    {
                        fileIndex = index;
                        break;
                    }

                    if (file.size < smallest)
                    {
                        fileIndex = index;
                        smallest = file.size;
                    }
                }
            }

            return fileIndex;
        }

        private bool ReadCompressedData(int fileIndex, byte[] dest, int destOffset, int offset, int size,
            BlockBuffer blockBuffer)
        {
            var localBlockBuffer = new BlockBuffer();

            // If a block buffer is not provided, allocate one.
            if (blockBuffer == null)
            {
                // Initialize the block buffer
                localBlockBuffer.index = -1;
                localBlockBuffer.compressedBuffer = new byte[myBlockSize];
                localBlockBuffer.expandedBuffer = new byte[myBlockSize];

                blockBuffer = localBlockBuffer;
            }

            // Read blocks as long as there is more data to read
            while (size > 0)
            {
                // Calculate which block is to be read and what data is to be read from it
                var blockIndex = offset / myBlockSize;
                var blockOffset = offset % myBlockSize;
                var bytesToRead = Math.Min(size, myBlockSize - blockOffset);

                // Read the block data unless it is the one currently in the buffer
                if (blockBuffer.index != blockIndex)
                {
                    ReadCompressedBlock(fileIndex, blockIndex, blockBuffer);
                }

                // Copy the data from the block buffer
                Buffer.BlockCopy(blockBuffer.expandedBuffer, blockOffset, dest, destOffset, bytesToRead);

                // Update the locations to account for the data read from the first block
                destOffset += bytesToRead;
                offset += bytesToRead;
                size -= bytesToRead;
            }

            return true;
        }

        private void ReadCompressedBlock(int fileIndex, int blockIndex, BlockBuffer blockBuffer)
        {
            var entry = myFileArray[fileIndex];
            var block = entry.blockSet[blockIndex];

            if (blockBuffer == null)
            {
                return;
            }

            {
                // Set the file position to the start of the block
                SetArchiveFilePointer(block.offset);

                // Read the compressed data
                myArchiveFileHandle.Read(blockBuffer.compressedBuffer, 0, block.compressedSize);
            }

            int destLen;

            {
                // Decompress the data into the output buffer
                destLen = block.compressedSize;
                Buffer.BlockCopy(blockBuffer.compressedBuffer, 0, blockBuffer.expandedBuffer, 0, destLen);
            }

            if (block.expandedSize != destLen)
            {
            }

            // Set the index of the block buffer to match the block just read
            blockBuffer.index = blockIndex;
        }

        private void SetArchiveFilePointer(long location)
        {
            if (myArchiveFileHandle == null)
            {
                return;
            }

            myArchiveFileHandle.Seek(location, SeekOrigin.Begin);
        }

        public override StreamReader OpenReader(string name, OpenFlags flags = OpenFlags.Buffered)
        {
            var index = FindFile(name);

            if (index < 0)
            {
                return null;
            }

            return new VirtualStreamReader(this, index);
        }

        public override StreamWriter OpenWriter(string name, OpenFlags flags = OpenFlags.DefaultWrite)
        {
            return null;
        }

        public override Result Stat(string path, out Entry entryOut)
        {
            entryOut = null;

            var index = FindFile(path);
            if (index < 0)
            {
                return Result.Error;
            }

            var fileEntry = GetFileEntry(index);
            var fileTime = DateTime.MinValue;

            try
            {
                fileTime = DateTime.FromBinary(fileEntry.fileTime);
            }
            catch (Exception)
            {
                // ignored
            }

            entryOut = new Entry();
            entryOut.SetAll(
                this,
                path,
                Entry.FlagValues.IsFile,
                fileTime,
                fileEntry.expandedSize);

            return Result.Ok;
        }

        private void InternalGetFiles(List<string> fileNames)
        {
            for (var iFile = 0; iFile < myFileArray.Count; ++iFile)
            {
                var file = myFileArray[iFile];

                if ((file.flags & FileFlag.Used) != 0)
                {
                    if (!string.IsNullOrEmpty(file.name))
                    {
                        fileNames.Add(file.name);
                    }
                }
            }
        }

        private void InternalGetFiles(List<string> fileNames, string path, bool recursive)
        {
            var pathLength = path.Length;

            var relativeNameBuilder = new StringBuilder();

            for (var iFile = 0; iFile < myFileArray.Count; ++iFile)
            {
                var file = myFileArray[iFile];

                if ((file.flags & FileFlag.Used) > 0)
                {
                    var fileName = file.name;
                    if (string.IsNullOrEmpty(fileName) || fileName.Length <= pathLength)
                    {
                        continue;
                    }

                    relativeNameBuilder.Length = 0;
                    relativeNameBuilder.Append(fileName);

                    if (fileName.StartsWith(path, StringComparison.Ordinal))
                    {
                        relativeNameBuilder.Remove(0, pathLength);
                        var relativeName = relativeNameBuilder.ToString();

                        if (!recursive)
                        {
                            if (relativeName.IndexOf('/') >= 0)
                            {
                                continue;
                            }
                        }

                        fileNames.Add(relativeName);
                    }
                }
            }
        }

        private void InternalGetFiles(List<string> fileNames, string path, string ext, bool recursive)
        {
            var pathLength = path.Length;
            var pathAndExtLength = ext.Length + pathLength;
            var relativeNameBuilder = new StringBuilder();

            for (var iFile = 0; iFile < myFileArray.Count; ++iFile)
            {
                var file = myFileArray[iFile];

                if ((file.flags & FileFlag.Used) > 0)
                {
                    var fileName = file.name;
                    if (string.IsNullOrEmpty(fileName) || fileName.Length <= pathAndExtLength)
                    {
                        continue;
                    }

                    relativeNameBuilder.Length = 0;
                    relativeNameBuilder.Append(fileName);

                    if (fileName.StartsWith(path, StringComparison.Ordinal) &&
                        fileName.EndsWith(ext, StringComparison.Ordinal))
                    {
                        relativeNameBuilder.Remove(0, pathLength);
                        var relativeName = relativeNameBuilder.ToString();


                        if (!recursive)
                        {
                            if (relativeName.IndexOf('/') >= 0)
                            {
                                continue;
                            }
                        }

                        fileNames.Add(relativeName);
                    }
                }
            }
        }

        public override void GetFiles(List<string> fileNames, string path, string searchPattern, bool recursive)
        {
            if (null == fileNames || null == path || null == searchPattern)
            {
                return;
            }

            var matchExt = searchPattern;

            while (matchExt.Length > 0 && matchExt[0] == '*' || matchExt[0] == '.')
            {
                matchExt = matchExt.Remove(0, 1);
            }

            if (path.Last() != '/')
            {
                path += '/';
            }

            if (matchExt.Length > 0 && path.Length > 0)
            {
                // Get the indices of the files matching the specified extension
                InternalGetFiles(fileNames, path, matchExt, recursive);
            }
            else if (path.Length > 0)
            {
                InternalGetFiles(fileNames, path, recursive);
            }
            else
            {
                InternalGetFiles(fileNames);
            }
        }
    }
}