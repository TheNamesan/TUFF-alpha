using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class DatabaseLoader : MonoBehaviour
    {
        private static Unit[] m_units;
        private static Job[] m_jobs;
        private static Skill[] m_skills;
        private static Command[] m_commands;
        private static Item[] m_items;
        private static KeyItem[] m_keyItems;
        private static Weapon[] m_weapons;
        private static Armor[] m_armors;
        private static State[] m_states;
        private static BattleAnimation[] m_animations;

        public static Unit[] units { get { InitializeDatabase(); return m_units; } }
        public static Job[] jobs { get { InitializeDatabase(); return m_jobs; } }
        public static Skill[] skills { get { InitializeDatabase(); return m_skills; } }
        public static Command[] commands { get { InitializeDatabase(); return m_commands; } }
        public static Item[] items { get { InitializeDatabase(); return m_items; } }
        public static KeyItem[] keyItems { get { InitializeDatabase(); return m_keyItems; } }
        public static Weapon[] weapons { get { InitializeDatabase(); return m_weapons; } }
        public static Armor[] armors { get { InitializeDatabase(); return m_armors; } }
        public static State[] states { get { InitializeDatabase(); return m_states; } }
        public static BattleAnimation[] animations { get { InitializeDatabase(); return m_animations; } }
        public static string battlePath = "Database/9Battles";
        public static string animationsPath = "Database/11Animations";
        public static string termsPath = "Database/12Terms/Terms";

        private static bool m_initialized = false;
        
        //protected static DatabaseLoader m_instance;
        public void Awake()
        {
            //if (m_instance != null)
            //{
            //    if (m_instance != this) Destroy(gameObject);
            //}
            //else
            //{
            //    AssignInstance(this);
            //}
            InitializeDatabase();
        }

        public static void InitializeDatabase()
        {
            if (m_initialized) return;
            m_units = Resources.LoadAll<Unit>("Database/0Units");
            m_jobs = Resources.LoadAll<Job>("Database/1Jobs");
            m_skills = Resources.LoadAll<Skill>("Database/2Skills");
            m_commands = Resources.LoadAll<Command>("Database/3Commands");
            m_items = Resources.LoadAll<Item>("Database/4Items");
            m_keyItems = Resources.LoadAll<KeyItem>("Database/5KeyItems");
            m_weapons = Resources.LoadAll<Weapon>("Database/6Weapons");
            m_armors = Resources.LoadAll<Armor>("Database/7Armors");
            m_states = Resources.LoadAll<State>("Database/10States");
            m_animations = Resources.LoadAll<BattleAnimation>(animationsPath);
            if (Application.isPlaying) AssignIDs();
            if (Application.isPlaying) m_initialized = true;
        }

        //protected static void AssignInstance(DatabaseLoader target)
        //{
        //    if (target == null) return;
        //    m_instance = target;
        //    if (target.gameObject)
        //        DontDestroyOnLoad(target.gameObject);
        //    m_instance.units = Resources.LoadAll<Unit>("Database/0Units");
        //    m_instance.jobs = Resources.LoadAll<Job>("Database/1Jobs");
        //    m_instance.skills = Resources.LoadAll<Skill>("Database/2Skills");
        //    m_instance.commands = Resources.LoadAll<Command>("Database/3Commands");
        //    m_instance.items = Resources.LoadAll<Item>("Database/4Items");
        //    m_instance.keyItems = Resources.LoadAll<KeyItem>("Database/5KeyItems");
        //    m_instance.weapons = Resources.LoadAll<Weapon>("Database/6Weapons");
        //    m_instance.armors = Resources.LoadAll<Armor>("Database/7Armors");
        //    m_instance.states = Resources.LoadAll<State>("Database/10States");
        //    m_instance.animations = Resources.LoadAll<BattleAnimation>(animationsPath);
        //    m_instance.AssignIDs();
        //}
        protected static void AssignIDs()
        {
            //for (int i = 0; i < m_units.Length; i++) { m_units[i].id = i; }
            //for (int i = 0; i < m_jobs.Length; i++) { m_jobs[i].id = i; }
            //for (int i = 0; i < m_skills.Length; i++) { m_skills[i].id = i; }
            //for (int i = 0; i < m_commands.Length; i++) { m_commands[i].id = i; }
            //for (int i = 0; i < m_items.Length; i++) { m_items[i].id = i; }
            //for (int i = 0; i < m_keyItems.Length; i++) { m_keyItems[i].id = i; }
            //for (int i = 0; i < m_weapons.Length; i++) { m_weapons[i].id = i; }
            //for (int i = 0; i < m_armors.Length; i++) { m_armors[i].id = i; }
            //for (int i = 0; i < m_states.Length; i++) { m_states[i].id = i; }
        }
        protected static object GetFromArray(object[] array, int index)
        {
            if (index < 0 || index >= array.Length) return null;
            return array[index];
        }
        public static Unit GetUnitFromID(int index) => (Unit)GetFromArray(units, index);
        public static Job GetJobFromID(int index) => (Job)GetFromArray(jobs, index);
        public static Skill GetSkillFromID(int index) => (Skill)GetFromArray(skills, index);
        public static Command GetCommandFromID(int index) => (Command)GetFromArray(commands, index);
        public static Item GetItemFromID(int index) => (Item)GetFromArray(items, index);
        public static KeyItem GetKeyItemFromID(int index) => (KeyItem)GetFromArray(keyItems, index);
        public static Weapon GetWeaponFromID(int index) => (Weapon)GetFromArray(weapons, index);
        public static Armor GetArmorFromID(int index) => (Armor)GetFromArray(armors, index);
        public static State GetStateFromID(int index) => (State)GetFromArray(states, index);

        public static int FindID(DatabaseElement databaseElement)
        {
            if (databaseElement is Unit unit) return System.Array.IndexOf(units, unit);
            if (databaseElement is Job job) return System.Array.IndexOf(jobs, job);
            if (databaseElement is Skill skill) return System.Array.IndexOf(skills, skill);
            if (databaseElement is Command command) return System.Array.IndexOf(commands, command);
            if (databaseElement is Item item) return System.Array.IndexOf(items, item);
            if (databaseElement is KeyItem keyItem) return System.Array.IndexOf(keyItems, keyItem);
            if (databaseElement is Weapon weapon) return System.Array.IndexOf(weapons, weapon);
            if (databaseElement is Armor armor) return System.Array.IndexOf(armors, armor);
            if (databaseElement is State state) return System.Array.IndexOf(states, state);
            return -1;
        }

        public static List<State> GetAllStatesOfType(StateType type)
        {
            var statesOfType = new List<State>();
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].stateType != type) continue;
                statesOfType.Add(states[i]);
            }
            return statesOfType;
        }
    }
}