// Amplify Color - Advanced Color Grading for Unity Pro
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AmplifyColorEffectEditorBase : Editor
{
	/*
	SerializedProperty tonemapper;
	SerializedProperty exposure;
	SerializedProperty linearWhitePoint;
	SerializedProperty useDithering;
 	*/

	SerializedProperty qualityLevel;
	SerializedProperty blendAmount;
	SerializedProperty lutTexture;
	SerializedProperty lutBlendTexture;
	SerializedProperty ghostColor;

    void OnEnable()
	{
		/*
		tonemapper = serializedObject.FindProperty( "Tonemapper" );
		exposure = serializedObject.FindProperty( "Exposure" );
		linearWhitePoint = serializedObject.FindProperty( "LinearWhitePoint" );
		useDithering = serializedObject.FindProperty( "ApplyDithering" );
		*/
		qualityLevel = serializedObject.FindProperty( "QualityLevel" );
		blendAmount = serializedObject.FindProperty( "BlendAmount" );
		lutTexture = serializedObject.FindProperty( "LutTexture" );
		lutBlendTexture = serializedObject.FindProperty( "LutBlendTexture" );
	    ghostColor = serializedObject.FindProperty("m_GhostColor");


        if ( !Application.isPlaying )
		{
			AmplifyColorBase effect = target as AmplifyColorBase;

			bool needsNewID = string.IsNullOrEmpty( effect.SharedInstanceID );
			if ( !needsNewID )
				needsNewID = FindClone( effect );

			if ( needsNewID )
			{
				effect.NewSharedInstanceID();
				EditorUtility.SetDirty( target );
			}
		}
	}

	bool FindClone( AmplifyColorBase effect )
	{
		GameObject effectPrefab = PrefabUtility.GetPrefabParent( effect.gameObject ) as GameObject;
		PrefabType effectPrefabType = PrefabUtility.GetPrefabType( effect.gameObject );
		bool effectIsPrefab = ( effectPrefabType != PrefabType.None && effectPrefabType != PrefabType.PrefabInstance );
		bool effectHasPrefab = ( effectPrefab != null );

		AmplifyColorBase[] all = Resources.FindObjectsOfTypeAll( typeof( AmplifyColorBase ) ) as AmplifyColorBase[];
		bool foundClone = false;

		foreach ( AmplifyColorBase other in all )
		{
			if ( other == effect || other.SharedInstanceID != effect.SharedInstanceID )
			{
				// skip: same effect or already have different ids
				continue;
			}

			GameObject otherPrefab = PrefabUtility.GetPrefabParent( other.gameObject ) as GameObject;
			PrefabType otherPrefabType = PrefabUtility.GetPrefabType( other.gameObject );
			bool otherIsPrefab = ( otherPrefabType != PrefabType.None && otherPrefabType != PrefabType.PrefabInstance );
			bool otherHasPrefab = ( otherPrefab != null );

			if ( otherIsPrefab && effectHasPrefab && effectPrefab == other.gameObject )
			{
				// skip: other is a prefab and effect's prefab is other
				continue;
			}

			if ( effectIsPrefab && otherHasPrefab && otherPrefab == effect.gameObject )
			{
				// skip: effect is a prefab and other's prefab is effect
				continue;
			}

			if ( !effectIsPrefab && !otherIsPrefab && effectHasPrefab && otherHasPrefab && effectPrefab == otherPrefab )
			{
				// skip: both aren't prefabs and both have the same parent prefab
				continue;
			}

			foundClone = true;
		}

		return foundClone;
	}

	void ToggleContextTitle( SerializedProperty prop, string title )
	{
		GUILayout.Space( 5 );
		GUILayout.BeginHorizontal();
		prop.boolValue = GUILayout.Toggle( prop.boolValue, "", GUILayout.Width( 15 ) );
		GUILayout.BeginVertical();
		GUILayout.Space( 3 );
		GUILayout.Label( title );
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		Camera ownerCamera = ( target as AmplifyColorBase ).GetComponent<Camera>();

		GUILayout.BeginVertical();

		GUILayout.Label( "Color Grading", EditorStyles.boldLabel );
		GUILayout.Space( -4 );
		// EditorGUILayout.PropertyField( qualityLevel );
		EditorGUILayout.PropertyField( blendAmount );
		EditorGUILayout.PropertyField( lutTexture );
		EditorGUILayout.PropertyField( lutBlendTexture );
		EditorGUILayout.PropertyField( ghostColor );

        serializedObject.ApplyModifiedProperties();
	}
}
