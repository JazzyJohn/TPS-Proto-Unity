using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
  Клавишы которые нужно добавить:
  skillActionKey - запуск умения
  selectSkill1, selectSkill2, selectSkill3, selectSkill4 - переключение на умение. 
  Добавляем столько, сколько у нас будет максимум умений у 1 персонажа.
*/

public class SkillConstants : MonoBehaviour
{

    public float[,,] engineerSkillStat;
    public float[,,] medicSkillStat;
    public float[,,] sniperSkillStat;
    public float[,,] stormTrooperSkillStat;
    public float cooldownOnChange = 0;  // кулдаун при смене скилла. В сек.

    private static SkillConstants s_Instance = null;
    
	void Awake() 
    {
                            // № умения / уровень / параметр
        engineerSkillStat     = new float[4, 5, 5];
	    medicSkillStat        = new float[4, 5, 5];
        sniperSkillStat       = new float[4, 5, 5];
        stormTrooperSkillStat = new float[4, 5, 5];        

        //-----------------------------------ИНЖЕНЕР-------------------------------------
        //-------------------------------------------------------------------------------
        // Установка турели

        // Экстренный ремонт

        // Стрельба деталями(

        // Лишние детали

        //-------------------------------------МЕДИК-------------------------------------
        //-------------------------------------------------------------------------------
        //Медик поблизости    
        //        Радиус             /      Откат/кулдаун в сек    /  Кол-во хп восстановления
        medicSkillStat[1, 1, 0] = 3;    medicSkillStat[1, 1, 1] = 0.2f; medicSkillStat[1, 1, 2] = 1;
        medicSkillStat[1, 2, 0] = 3.5f; medicSkillStat[1, 2, 1] = 0.2f; medicSkillStat[1, 2, 2] = 1;
        // Экстренная реанимация
        medicSkillStat[2, 1, 0] = 0; medicSkillStat[2, 1, 1] = 0;
        medicSkillStat[2, 2, 0] = 0; medicSkillStat[2, 2, 1] = 0;

        // Смена полярности
        //        Радиус             /      Откат/кулдаун в сек    /  
        medicSkillStat[3, 1, 0] = 3;    medicSkillStat[3, 1, 1] = 2; 
        medicSkillStat[3, 2, 0] = 3.5f; medicSkillStat[3, 2, 1] = 2; 

        // Полевой хируг


        //-----------------------------------СНАЙПЕР-------------------------------------
        //-------------------------------------------------------------------------------
        // Невидимость

        // Даю наводку 

        // Открытая местность

        // На охоте

        //----------------------------------ШТУРМОВИК------------------------------------
        //-------------------------------------------------------------------------------
        // Одной крови

        // Боевые стимуляторы 

        // Делай как я

        // Рикошет от шлема 



	}

    public static SkillConstants instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(SkillConstants)) as SkillConstants;
            }

            /*
            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                GameObject obj = new GameObject("SkillConstants");
                s_Instance = obj.AddComponent(typeof(SkillConstants)) as SkillConstants;
                Debug.Log("Could not locate an SkillConstants object.  AManager was Generated Automaticly.");
            }//*/

            return s_Instance;
        }
    }

}
