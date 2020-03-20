using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;

namespace FluxEditor
{
	public class AnimationWindowProxy {

		private static Type ANIMATION_WINDOW_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationWindow");
		private static Type ANIMATION_WINDOW_STATE_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditorInternal.AnimationWindowState");
		private static Type ANIMATION_SELECTION_TYPE = typeof(EditorWindow).Assembly.GetType("UnityEditor.AnimationSelection");

		private static EditorWindow _animationWindow = null;

		public static EditorWindow AnimationWindow {
			get	{
				if( _animationWindow == null )
					_animationWindow = FUtility.GetWindowIfExists( ANIMATION_WINDOW_TYPE );
				return _animationWindow;
			}
		}

		public static EditorWindow OpenAnimationWindow()
		{
			if( _animationWindow == null )
				_animationWindow = EditorWindow.GetWindow( ANIMATION_WINDOW_TYPE );
			return _animationWindow;
		}

		#region AnimationWindow variables

		private static PropertyInfo _stateProperty = null;
		private static PropertyInfo StateProperty
		{
			get{
				if( _stateProperty == null )
					_stateProperty = ANIMATION_WINDOW_TYPE.GetProperty("state", BindingFlags.Instance | BindingFlags.Public);
				return _stateProperty;
			}
		}

		private static FieldInfo _selectedAnimationField = null;
		private static FieldInfo SelectedAnimationField {
			get {
				if( _selectedAnimationField == null )
					_selectedAnimationField = ANIMATION_WINDOW_TYPE.GetField("m_Selected");
				return _selectedAnimationField;
			}
		}

		private static MethodInfo _beginAnimationMode = null;
		public static MethodInfo BeginAnimationMode
		{
			get {
				if( _beginAnimationMode == null )
					_beginAnimationMode = ANIMATION_WINDOW_TYPE.GetMethod("BeginAnimationMode", BindingFlags.Instance | BindingFlags.Public, null, new Type[]{typeof(bool)}, null );
				return _beginAnimationMode;
			}
		}

		private static MethodInfo _previewFrame = null;
		public static MethodInfo PreviewFrame
		{
			get {
				if( _previewFrame == null )
					_previewFrame = ANIMATION_WINDOW_TYPE.GetMethod("PreviewFrame", BindingFlags.Instance | BindingFlags.Public, null, new Type[]{typeof(int)}, null );
				return _previewFrame;
			}
		}

		#endregion

		#region AnimationWindowState variables
		private static FieldInfo _timeField = null;
		private static FieldInfo TimeField { 
			get {
				if( _timeField == null )
					_timeField = ANIMATION_WINDOW_STATE_TYPE.GetField("m_PlayTime");
				return _timeField;
			}
		}

		private static FieldInfo _frameField = null;
		private static FieldInfo FrameField {
			get {
				if( _frameField == null )
					_frameField = ANIMATION_WINDOW_STATE_TYPE.GetField("m_Frame");
				return _frameField;
			}
		}
		#endregion

		#region AnimationSelection variables
		private static MethodInfo _chooseClipMethod = null;
		private static MethodInfo ChooseClipMethod {
			get {
				if( _chooseClipMethod == null )
					_chooseClipMethod = ANIMATION_SELECTION_TYPE.GetMethod( "ChooseClip", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]{typeof(AnimationClip)}, null );
				return _chooseClipMethod;
			}
		}
		#endregion

		public static void StartAnimationMode()
		{
//			MethodInfo onSelectionChange = ANIMATION_WINDOW_TYPE.GetMethod( "OnSelectionChange", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null );
//			onSelectionChange.Invoke(AnimationWindow, null);
//			object[] selectedAnimation = (object[])SelectedAnimationField.GetValue( AnimationWindow );
//			Transform t = Selection.activeTransform;
//			Selection.activeTransform = null;
//			Selection.activeTransform = t;
//			MethodInfo reenterAnimationMode = ANIMATION_WINDOW_TYPE.GetMethod( "ReEnterAnimationMode", BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null );
//			reenterAnimationMode.Invoke( AnimationWindow, null );
			BeginAnimationMode.Invoke( AnimationWindow, new object[]{false} );
		}

		private static object GetState()
		{
			return StateProperty.GetValue( AnimationWindow, null );
		}

		public static void SetCurrentFrame( int frame, float time )
		{
			if( AnimationWindow == null )
				return;

			object state = GetState();

			TimeField.SetValue( state, time );
			FrameField.SetValue( state, frame );

			PreviewFrame.Invoke( AnimationWindow, new object[]{frame} );

			_animationWindow.Repaint();
		}

		public static int GetCurrentFrame()
		{
			if( AnimationWindow == null )
				return -1;

			return (int)FrameField.GetValue( GetState() );
		}

		public static void SelectAnimationClip( AnimationClip clip )
		{
			if( AnimationWindow == null || clip == null )
				return;

			AnimationClip[] clips = AnimationUtility.GetAnimationClips(Selection.activeGameObject);

			int index = 0;
			for( ; index != clips.Length; ++index )
			{
				if( clips[index] == clip )
					break;
			}


			if( index == clips.Length )
			{
				// didn't find
				Debug.LogError("Couldn't find clip " + clip.name);
			}
			else
			{
				// found
				object[] selectedAnimation = (object[])SelectedAnimationField.GetValue( AnimationWindow );
				if( selectedAnimation.Length > 0 )
					ChooseClipMethod.Invoke( selectedAnimation[0], new object[]{clip} );
			}
		}
	}
}
