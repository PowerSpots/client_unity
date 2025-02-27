using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Flux
{
    /**
	 * @brief A timeline is an object that holds tracks that pertain to 
	 * a specific object, aka Owner. When evaluated this timeline will 
	 * run events which most of the times will directly affect the Owner.
	 * @sa FSquence, FTrack, FEvent
	 */
    public class FTimeline : FObject//, ISerializationCallbackReceiver
	{

		// To which Sequence this timeline belongs to
//		[SerializeField]
//		private FSequence _sequence;
		[SerializeField]
		private FContainer _container = null;
		public FContainer Container { get { return _container; } }

		// Which object is the owner of this timeline
		[SerializeField]
		private Transform _owner = null;

		// What's the path to the _owner, used for serialization purposes in prefabs
		[SerializeField]
		private string _ownerPath = null;
		public string OwnerPath { get { return _ownerPath; } }

        [SerializeField]
        private string _ownerTag = null;
        public string OwnerTag { get { return _ownerTag; } }

//		[SerializeField]
//		[HideInInspector]
//		private bool _isGlobal = false;
//		/// @brief IsGlobal defines if this is the global timeline of the sequence. 
//		/// There can only be one of these, and they aren't processed at runtime! It
//		/// is used for things like the comment track.
//		public bool IsGlobal { get { return _isGlobal; } set { _isGlobal = true; } }

		public override FSequence Sequence { get { return _container.Sequence; } }

		public override Transform Owner { get { return _owner; } }

		public void Awake()
		{
			if( _owner == null && !string.IsNullOrEmpty(_ownerPath) )
				_owner = transform.Find( _ownerPath );
		}

		/// @brief Sets the owner of this timeline
		public void SetOwner( Transform owner ) { 
			_owner = owner; 
			if( _owner != null ) 
				name = _owner.name;
			OnValidate();
			if( Container != null && Sequence.IsInit )
				Init();
		}

		// Sets sequence reference. It is meant to be called from FSequence, not publicly accessable.
//		internal void SetSequence( FSequence sequence )
//		{
//			if( _sequence == sequence )
//				return;
//			_sequence = sequence;
//			if( _sequence )
//			{
//				transform.parent = _sequence.TimelineContainer;
//			}
//			else
//			{
//				transform.parent = null;
//			}
//		}
		internal void SetContainer( FContainer container )
		{
			if( _container == container )
				return;
			_container = container;
			if( _container )
				transform.parent = container.transform;
			else
				transform.parent = null;
			OnValidate();
		}

		// tracks
		[SerializeField]
		private List<FTrack> _tracks = new List<FTrack>();
		/// @brief Get the tracks inside this timeline
		public List<FTrack> Tracks { get { return _tracks; } }

		/**
		 * @brief Create a new timeline
		 * @param owner Owner of this timeline
		 */
		public static FTimeline Create( Transform owner )
		{
			GameObject go = new GameObject(owner.name);
			FTimeline timeline = go.AddComponent<FTimeline>();
			timeline.SetOwner( owner );

			return timeline;
		}

		/**
		 * @brief Initializes the timeline.
		 * @sa FSequence.Init
		 */
		public override void Init()
		{
			if( _owner == null )
				Awake();

			enabled = Owner != null;

			if( !enabled )
				return;

			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].Init();
		}

		/**
		 * @brief Pauses the timeline.
		 * @sa FSequence.Pause
		 */
		public void Pause()
		{
			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].Pause();
		}

		/**
		 * @brief Resumes the timeline after it has been paused.
		 * @sa FSequence.Play, FSequence.Pause
		 */
		public void Resume()
		{
			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].Resume();
		}

		/**
		 * @brief Stops the timeline, i.e. brings it back to the start.
		 * @sa FSequence.Stop
		 */
		public override void Stop()
		{
		    if (null == Owner)
		    {
                return;
		    }

			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].Stop();
		}

        /**
         * @brief Preloads the timeline before it will be used
         * @sa FSequence.Preload
        */
        public override void Preload()
        {           
            for (int i = 0; i != _tracks.Count; ++i)
                _tracks[i].Preload();
        }

		/// @brief Returns if the timeline doesn't have any events.
		public bool IsEmpty()
		{
			foreach( FTrack track in _tracks )
			{
				if( !track.IsEmpty() )
					return false;
			}

			return true;
		}

		/**
		 * @brief Adds a new track to the timeline
		 * @param range A track by default is added with 1 event
		 * @T Event type that the track will hold
		 * @sa RemoveTrack
		 */
		public FTrack Add<T>( FrameRange range ) where T : FEvent
		{
			FTrack track = FTrack.Create<T>();

			Add( track );
//			int id = _tracks.Count;
//
//			_tracks.Add( track );
//
//			track.SetTimeline( this );
//			track.SetId( id );
//
//			if( !Sequence.IsStopped )
//				track.Init();

			FEvent evt = FEvent.Create<T>( range );

			track.Add( evt );

			return track;
		}

        /// <summary>
        /// 增加一个空的track，不要默认的那个event
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FTrack AddEmptyTrack<T>() where T : FEvent
        {
            FTrack track = FTrack.Create<T>();

            Add(track);

            return track;
        }

		public void Add( FTrack track )
		{
			int id = _tracks.Count;

			_tracks.Add( track );

			track.SetTimeline( this );
			track.SetId( id );

			if( !Sequence.IsStopped )
				track.Init();
		}

		/**
		 * @brief Removes a track
		 * @param track Track to remove
		 * @sa AddTrack
		 */
		public void Remove( FTrack track )
		{
			if( _tracks.Remove( track ) )
			{
				track.SetTimeline( null );

				UpdateTrackIds();
			}
		}

		/**
		 * @brief Updates the tracks of this timeline
		 * @param frame Current frame of the sequence
		 * @param time Current time of the sequence
		 */
		public void UpdateTracks( int frame, float time )
		{
			for( int i = 0; i != _tracks.Count; ++i )
			{
				if( !_tracks[i].enabled ) continue;
				_tracks[i].UpdateEvents( frame, time );
			}
		}

		public void UpdateTracksEditor( int frame, float time )
		{
			for( int i = 0; i != _tracks.Count; ++i )
			{
				if( !_tracks[i].enabled || (_tracks[i].CanTogglePreview && !_tracks[i].CanPreview) ) continue;
				_tracks[i].UpdateEventsEditor( frame, time );
			}
		}

		/**
		 * @brief Rebuilds a timeline. To be called when the hierarchy changes,
		 * ie tracks get added / deleted.
		 */
		public void Rebuild()
		{
			Transform t = transform;
			_tracks.Clear();

			for( int i = 0; i != t.childCount; ++i )
			{
				FTrack track = t.GetChild(i).GetComponent<FTrack>();
				if( track )
				{
					_tracks.Add( track );
					track.SetTimeline( this );
					track.Rebuild();
				}
			}

			UpdateTrackIds();
		}

		// updates the track ids
		private void UpdateTrackIds()
		{
			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].SetId( i );
		}
		/*
		/// @brief Method that serializes the owner path
		public void OnBeforeSerialize()
		{
			Debug.Log("OnBeforeSerialize");
			if( _owner != null && !Application.isPlaying )
				_ownerPath = GetTransformPath( _owner );
		}

		/// @brief Method that deserializes the owner path
		public void OnAfterDeserialize()
		{
			Debug.Log("OnAfterDeserialize");
			if( _owner == null && !string.IsNullOrEmpty(_ownerPath) )
				_owner = transform.Find( _ownerPath );
		}*/

		protected virtual void OnValidate()
		{
			if( _owner != null )
				_ownerPath = GetTransformPath( _owner );
		}

		// helper function to get the transform path of a specific transform t
		private string GetTransformPath( Transform t )
		{
			StringBuilder sb = new StringBuilder(t.name);

			if( Container == null || Sequence == null )
				return string.Empty;
			Transform sequenceTransform = Sequence.transform;

			t = t.parent;

			while( t != null )
			{
				if( t == sequenceTransform )
					return sb.ToString();

				sb.Insert( 0, string.Format("{0}/", t.name) );

				t = t.parent;
			}

			sb.Insert( 0, '/' );

			return sb.ToString();
		}
	}
}
