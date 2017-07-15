using Coding4Fun.ScriptTD.Engine.Logic;

namespace Coding4Fun.ScriptTD.Engine.Data.Abstracts
{
    public interface IAmmo
    {
        void Update(float elapsedSeconds, TowerData data, ref GameSession session);
        void Reset();
    }
}
