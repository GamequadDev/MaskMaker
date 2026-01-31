using UnityEditor;

public class BuildScript {
    static string[] Scenes = { "Assets/Scenes/Store/MainStore.unity" };

    public static void BuildWebGL() {
        BuildPipeline.BuildPlayer(Scenes, "Builds/WebGL", BuildTarget.WebGL, BuildOptions.None);
    }

    public static void BuildWindows() {
        // Zauważ, że tu podajemy plik .exe, a nie tylko folder
        BuildPipeline.BuildPlayer(Scenes, "Builds/Windows/MojaGra.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }
}
