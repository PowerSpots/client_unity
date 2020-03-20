module("Gankx.Config", package.seeall)

UI =
{
    panelUnloadCacheCount = CS.PlatformExport.IsIOS() and 0 or 10,
}