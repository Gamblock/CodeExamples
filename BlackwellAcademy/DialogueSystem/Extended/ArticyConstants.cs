namespace UnlockGames.BA.Core.DialogueSystem
{
    public static class ArticyConstants
    {
        //Instruction Title
        public const string FieldInstructionTimelineStop = "InstructionTimelineStop";
        public const string FieldInstructionTimeline = "InstructionTimeline";
        public const string FieldInstructionSequence = "InstructionSequence";

        //Field Title
        public const string FieldTitleArticyId = "Articy Id";
        public const string FieldTitleArticyTechnicalName = "Technical Name";
        public const string FieldTitleArticyName = "Name";
        public const string FieldTitleDestinationId = "destinationArticyId";
        public const string FieldTitleTechnicalName = "technicalName";
        public const string FieldTitleOwnerSlot = "OwnerSlot";
        public const string FieldTitleTechnicalPrefixName = "technicalPrefixName";
        public const string FieldTitleTechnicalPostfixName = "technicalPostfixName";
        public const string FieldTitleTitle = "Title";
        public const string FieldTitleNodeName = "NodeName";
        public const string FieldTitlePrefixSubtable = "SUBTABLE__";
        public const string FieldTitleActorArticyId = "ActorArticyId";
        public const string FieldTitleAllActorsArticyIds = "AllActorsArticyIds";
        public const string FieldTitleQuestGroupId = "QuestGroupId";
        public const string FieldTitleQuestGroupStrip = "QuestGroup";
        public const string FieldTitleDialogue = "DialogueSlot";
        public const string FieldTitleQuestList = "QuestList";
        
        public const string FieldTitleType = "Type";
        //Articy Id
        public const string IdEmpty = "0x0000000000000000";
        
        
        public enum CompareType
        {
            None = 0,
            Less = 1,
            LessEqually = 2,
            Equally = 3,
            MoreEqually = 4,
            More = 5
        }
    }
}
