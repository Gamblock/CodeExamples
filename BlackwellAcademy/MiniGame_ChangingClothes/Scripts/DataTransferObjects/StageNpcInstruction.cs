
namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class StageNpcInstruction
    {
        public string npcToStage;
        public string animationToPlay;
        public string cameraPreset;

        public StageNpcInstruction(string npcToStage, string _, string animationToPlay, string cameraPreset)
        {
            this.npcToStage = npcToStage;
            this.animationToPlay = animationToPlay;
            this.cameraPreset = cameraPreset;
        }
    }
}

