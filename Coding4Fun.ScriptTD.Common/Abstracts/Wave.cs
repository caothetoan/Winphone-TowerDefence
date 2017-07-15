namespace Coding4Fun.ScriptTD.Common.Abstracts
{
    public abstract class Wave
    {
        public float HealthModifier;
        public float WorthModifier;

        public float WarmupTime = 10; // todo: Either keep this as a default, or load from xml - in seconds
    }
}
