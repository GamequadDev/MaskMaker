using UnityEditor;

public class BuildScript {
    static string[] Scenes = { "Assets/Scenes/Store/MainStore1.unity" };

    public static void BuildWebGL() {
        BuildPipeline.BuildPlayer(Scenes, "Builds/WebGL", BuildTarget.WebGL, BuildOptions.None);
    }

    public static void BuildWindows() {
        // Zauważ, że tu podajemy plik .exe, a nie tylko folder
        BuildPipeline.BuildPlayer(Scenes, "Builds/Windows/MojaGra.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
    }

    public static void BuildLinux() {
        // Dla Linuxa buildem jest plik wykonywalny (zazwyczaj bez rozszerzenia lub .x86_64)
        BuildPipeline.BuildPlayer(Scenes, "Builds/Linux/MojaGra.x86_64", BuildTarget.StandaloneLinux64, BuildOptions.None);
    }
}
