namespace AdbGame.Model
{
    public class StepModel
    {
        public string GameName { get; set; }
        public string GameInstance { get; set; }
        public string StepName { get; set; }
        public int XShift { get; set; }
        public int YShift { get; set; }
        public string MatchMode { get; set; } = "TemplateMatch";
    }

    public enum MatchMode
    {
        TemplateMatch,
        OCR
    }
}
