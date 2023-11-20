using System;
using UnityEngine;
using UnityEditor;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
public class SimpleLitLJShader : BaseShaderGUI
{ 

    private SimpleLitGUI.SimpleLitProperties shadingModelProperties;

    protected MaterialProperty BumpMapProp { get; set; }
    protected MaterialProperty BumpMap1Prop{ get; set; }
    protected MaterialProperty BumpMap2Prop { get; set; }
    protected MaterialProperty BumpMap3Prop { get; set; }
    protected MaterialProperty baseMap1Prop { get; set; }
    protected MaterialProperty baseMap2Prop { get; set; }
    protected MaterialProperty baseMap3Prop { get; set; }
    protected MaterialProperty SplatmapOnProp { get; set; }
    protected MaterialProperty SplatmapProp { get; set; }
    protected MaterialProperty InholeProp { get; set; }

    private bool Splatmapshow = true;
    private bool SplatmapshowMap = true;
    private bool Inhole;
     private int queueOffsetRange;
        //private int queueOffsetRange = 50;
        //public int queueValue;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            base.OnGUI(materialEditor, properties); // 调用基类的OnGUI方法

        }


        public override void FindProperties(MaterialProperty[] properties)
        {

            base.FindProperties(properties);
            shadingModelProperties = new SimpleLitGUI.SimpleLitProperties(properties);
//=====================我的覆写参数===========================================================================
            SplatmapProp = FindProperty("_Splatmap", properties, false);
            //--------------------------------------------------------
            BumpMapProp = FindProperty("_BumpMap", properties, false);
            BumpMap1Prop = FindProperty("_BumpMap1", properties, false);
            BumpMap2Prop = FindProperty("_BumpMap2", properties, false);
            BumpMap3Prop = FindProperty("_BumpMap3", properties, false);
            //--------------------------------------------------------
            baseMapProp = FindProperty("_BaseMap", properties, false);
            baseMap1Prop = FindProperty("_BaseMap1", properties, false);
            baseMap2Prop = FindProperty("_BaseMap2", properties, false);
            baseMap3Prop = FindProperty("_BaseMap3", properties, false);
            SplatmapOnProp = FindProperty("_SplatmapOn", properties, false);
         
        }


 //========================我的类===============================================       
        public static class LJStyles
        {
            public static GUIContent Splatmap = new GUIContent("Splatmap Texture");
            //-----------------------------------------------
            public static GUIContent baseMap = new GUIContent("Texture1");
            public static GUIContent BumpMap = new GUIContent("BumpMap1");
            public static GUIContent baseMap1 = new GUIContent("Texture2");
            public static GUIContent BumpMap1 = new GUIContent("BumpMap2");
            public static GUIContent baseMap2 = new GUIContent("Texture3");
            public static GUIContent BumpMap2 = new GUIContent("BumpMap3");
            public static GUIContent baseMap3 = new GUIContent("Texture4");
            public static GUIContent BumpMap3 = new GUIContent("BumpMap4");
            public static GUIContent SplatmapOn = new GUIContent("Splatmap");

        }
//========================我的类===============================================       

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, SimpleLitGUI.SetMaterialKeywords);
        }

        // material main surface options
        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // Use default labelWidth
            EditorGUIUtility.labelWidth = 0f;

            base.DrawSurfaceOptions(material);
        }

        // material main surface inputs
        public override void DrawSurfaceInputs(Material material)
        {

            if(SplatmapshowMap == false)
            { 
            base.DrawSurfaceInputs(material);
             DrawTileOffset(materialEditor, baseMapProp);
            }
            SimpleLitGUI.Inputs(shadingModelProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
//========================================================
Rect rectSplatmapshow = EditorGUILayout.BeginVertical();
Color colorSplatmapshow = Color.black; // 设置范围框的颜色（使用RGB值）
EditorGUI.DrawRect(rectSplatmapshow, colorSplatmapshow);
        Splatmapshow = EditorGUILayout.Foldout(Splatmapshow, "Terrain");
EditorGUILayout.EndVertical();
            SplatmapshowMap = SplatmapOnProp.floatValue > 0;
            if(Splatmapshow)
            {   
                materialEditor.ShaderProperty(SplatmapOnProp, LJStyles.SplatmapOn.text, (int)MaterialProperty.PropType.Float);
                if(SplatmapshowMap)
                {
                materialEditor.TexturePropertySingleLine(LJStyles.Splatmap, SplatmapProp);                            
Rect lineRect = EditorGUILayout.GetControlRect(false, 1);
EditorGUI.DrawRect(lineRect,new Color(0.0f,0.0f,0.0f, 0.3f));
                materialEditor.TexturePropertySingleLine(LJStyles.baseMap, baseMapProp);
                materialEditor.TexturePropertySingleLine(LJStyles.BumpMap, BumpMapProp);    
                DrawTileOffset(materialEditor, baseMapProp);
Rect lineRect2 = EditorGUILayout.GetControlRect(false, 1);
EditorGUI.DrawRect(lineRect2,new Color(0.0f,0.0f,0.0f, 0.3f));
                materialEditor.TexturePropertySingleLine(LJStyles.baseMap1, baseMap1Prop);
                materialEditor.TexturePropertySingleLine(LJStyles.BumpMap1, BumpMap1Prop);  
                DrawTileOffset(materialEditor, baseMap1Prop);
Rect lineRect3 = EditorGUILayout.GetControlRect(false, 1);
EditorGUI.DrawRect(lineRect3,new Color(0.0f,0.0f,0.0f, 0.3f));
                materialEditor.TexturePropertySingleLine(LJStyles.baseMap2, baseMap2Prop);
                materialEditor.TexturePropertySingleLine(LJStyles.BumpMap2, BumpMap2Prop);  
                DrawTileOffset(materialEditor, baseMap2Prop);
Rect lineRect4 = EditorGUILayout.GetControlRect(false, 1);
EditorGUI.DrawRect(lineRect4,new Color(0.0f,0.0f,0.0f, 0.3f));
                materialEditor.TexturePropertySingleLine(LJStyles.baseMap3, baseMap3Prop);
                materialEditor.TexturePropertySingleLine(LJStyles.BumpMap3, BumpMap3Prop);
                DrawTileOffset(materialEditor, baseMap3Prop);  
                }

            
            }

//=============================================================

        }

        public override void DrawAdvancedOptions(Material material)
        {

            SimpleLitGUI.Advanced(shadingModelProperties);
            
            Material targetMat = materialEditor.target as Material;            
            EditorGUILayout.LabelField("*** Surface:Opaque , Sorting : Inhole:-452 , other :0");
            DrawQueueOffsetField();
            
           // base.DrawAdvancedOptions(material);

            materialEditor.EnableInstancingField();
        
            EditorGUILayout.LabelField("Render Queue: " + targetMat.renderQueue);
        }

        public new void DrawQueueOffsetField()
        {
            queueOffsetRange = 1100; 
            if (queueOffsetProp != null)         
            materialEditor.IntSliderShaderProperty(queueOffsetProp, -queueOffsetRange, queueOffsetRange, Styles.queueSlider);
        }
        

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

        }


}



}