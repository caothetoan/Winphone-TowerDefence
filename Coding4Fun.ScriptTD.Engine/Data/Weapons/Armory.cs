using System;
using System.Collections.Generic;

namespace Coding4Fun.ScriptTD.Engine.Data.Weapons
{
    public static class Armory
    {
        private static readonly Dictionary<string, Type> Weapons = new Dictionary<string, Type>();

        public static void AddWeaponType(string name, Type type)
        {
            string n = name.ToUpper();

            if (!Weapons.ContainsKey(n))
                Weapons.Add(n, type);
        }

        public static Type GetWeaponType(string name)
        {
            var n = name.ToUpper();

            if (Weapons.ContainsKey(n))
                return Weapons[n];

            return null;
        }
    }
}
