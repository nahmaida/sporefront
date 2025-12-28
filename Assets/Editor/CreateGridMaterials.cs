// Assets/Editor/CreateGridMaterials.cs
using UnityEditor;
using UnityEngine;

public class CreateGridMaterials : EditorWindow
{
    [MenuItem("Tools/Create Grid Materials")]
    static void CreateMaterials()
    {
        // Создаем папку если нет
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        // Материал для союзников (синий)
        Material allyMat = new Material(Shader.Find("Standard"));
        allyMat.name = "AllyZoneMaterial";
        allyMat.SetFloat("_Mode", 3); // Режим Transparent
        allyMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        allyMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        allyMat.SetInt("_ZWrite", 0);
        allyMat.DisableKeyword("_ALPHATEST_ON");
        allyMat.EnableKeyword("_ALPHABLEND_ON");
        allyMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        allyMat.renderQueue = 3000;
        allyMat.color = new Color(0, 0, 1, 0.3f);
        AssetDatabase.CreateAsset(allyMat, "Assets/Materials/AllyZoneMaterial.mat");

        // Материал для врагов (красный)
        Material enemyMat = new Material(Shader.Find("Standard"));
        enemyMat.name = "EnemyZoneMaterial";
        enemyMat.SetFloat("_Mode", 3);
        enemyMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        enemyMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        enemyMat.SetInt("_ZWrite", 0);
        enemyMat.DisableKeyword("_ALPHATEST_ON");
        enemyMat.EnableKeyword("_ALPHABLEND_ON");
        enemyMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        enemyMat.renderQueue = 3000;
        enemyMat.color = new Color(1, 0, 0, 0.3f);
        AssetDatabase.CreateAsset(enemyMat, "Assets/Materials/EnemyZoneMaterial.mat");

        // Материал для нейтральных клеток (серый)
        Material neutralMat = new Material(Shader.Find("Standard"));
        neutralMat.name = "NeutralZoneMaterial";
        neutralMat.SetFloat("_Mode", 3);
        neutralMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        neutralMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        neutralMat.SetInt("_ZWrite", 0);
        neutralMat.DisableKeyword("_ALPHATEST_ON");
        neutralMat.EnableKeyword("_ALPHABLEND_ON");
        neutralMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        neutralMat.renderQueue = 3000;
        neutralMat.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
        AssetDatabase.CreateAsset(neutralMat, "Assets/Materials/NeutralZoneMaterial.mat");

        // Материал для подсветки (желтый)
        Material highlightMat = new Material(Shader.Find("Standard"));
        highlightMat.name = "HighlightMaterial";
        highlightMat.SetFloat("_Mode", 3);
        highlightMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        highlightMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        highlightMat.SetInt("_ZWrite", 0);
        highlightMat.DisableKeyword("_ALPHATEST_ON");
        highlightMat.EnableKeyword("_ALPHABLEND_ON");
        highlightMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        highlightMat.renderQueue = 3000;
        highlightMat.color = new Color(1, 1, 0, 0.5f);
        AssetDatabase.CreateAsset(highlightMat, "Assets/Materials/HighlightMaterial.mat");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Материалы созданы успешно!");
    }
}