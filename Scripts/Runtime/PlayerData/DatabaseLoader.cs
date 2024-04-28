using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public class DatabaseLoader : MonoBehaviour
    {
        public Unit[] units;
        public Job[] jobs;
        public Skill[] skills;
        public Command[] commands;
        public Item[] items;
        public KeyItem[] keyItems;
        public Weapon[] weapons;
        public Armor[] armors;
        public State[] states;
        public BattleAnimation[] animations;
        public static string battlePath = "Database/9Battles";
        public static string animationsPath = "Database/11Animations";
        public static string termsPath = "Database/12Terms/Terms";
        public static DatabaseLoader instance
        {
            get
            {
                if (!GameManager.instance) return null;
                return GameManager.instance.databaseLoader;
                //if (m_instance == null)
                //{
                //    if (GameManager.instance == null) return null;
                //    AssignInstance(GameManager.instance.GetComponentInChildren<DatabaseLoader>());
                //}

                //return m_instance;
            }
        }
        private bool m_initialized = false;
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

        public void InitializeDatabase()
        {
            if (m_initialized) return;
            units = Resources.LoadAll<Unit>("Database/0Units");
            jobs = Resources.LoadAll<Job>("Database/1Jobs");
            skills = Resources.LoadAll<Skill>("Database/2Skills");
            commands = Resources.LoadAll<Command>("Database/3Commands");
            items = Resources.LoadAll<Item>("Database/4Items");
            keyItems = Resources.LoadAll<KeyItem>("Database/5KeyItems");
            weapons = Resources.LoadAll<Weapon>("Database/6Weapons");
            armors = Resources.LoadAll<Armor>("Database/7Armors");
            states = Resources.LoadAll<State>("Database/10States");
            animations = Resources.LoadAll<BattleAnimation>(animationsPath);
            AssignIDs();
            m_initialized = true;
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
        protected void AssignIDs()
        {
            for (int i = 0; i < units.Length; i++) { units[i].id = i; }
            for (int i = 0; i < jobs.Length; i++) { jobs[i].id = i; }
            for (int i = 0; i < skills.Length; i++) { skills[i].id = i; }
            for (int i = 0; i < commands.Length; i++) { commands[i].id = i; }
            for (int i = 0; i < items.Length; i++) { items[i].id = i; }
            for (int i = 0; i < keyItems.Length; i++) { keyItems[i].id = i; }
            for (int i = 0; i < weapons.Length; i++) { weapons[i].id = i; }
            for (int i = 0; i < armors.Length; i++) { armors[i].id = i; }
            for (int i = 0; i < states.Length; i++) { states[i].id = i; }
        }
        protected object GetFromArray(object[] array, int index)
        {
            if (index < 0 || index >= array.Length) return null;
            return array[index];
        }
        public Unit GetUnitFromID(int index) => (Unit)GetFromArray(units, index);
        public Job GetJobFromID(int index) => (Job)GetFromArray(jobs, index);
        public Skill GetSkillFromID(int index) => (Skill)GetFromArray(skills, index);
        public Command GetCommandFromID(int index) => (Command)GetFromArray(commands, index);
        public Item GetItemFromID(int index) => (Item)GetFromArray(items, index);
        public KeyItem GetKeyItemFromID(int index) => (KeyItem)GetFromArray(keyItems, index);
        public Weapon GetWeaponFromID(int index) => (Weapon)GetFromArray(weapons, index);
        public Armor GetArmorFromID(int index) => (Armor)GetFromArray(armors, index);
        public State GetStateFromID(int index) => (State)GetFromArray(states, index);


        public List<State> GetAllStatesOfType(StateType type)
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