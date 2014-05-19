using UnityEngine;
using System.Collections;

public class basicSkill : MonoBehaviour
{
    public string  selectSkillKey = "selectSkill1";   // кнопка переключающее умение (selectSkill# где # = 1-4)
    public bool    isSelected = false;                // выбранно ли умение
    public Vector2 cooldownSkill;                     // откат умения. х - сколько осталось до отката, у - время отката 
    public float   cooldownOnChange;                  // доб время кулдауна при смене скилла
    public int     levelSkill = 1;                    // уровень умения
    public bool    isPassive  = false;                // Пассивный ли навык? Запускать ли автоматом?
    
    private PhotonView photonView;

	void Start () 
    {
        //if (isPlayerPremium)  // добавить код разграничивающий получение констант в зависимости от наличия премиума учетки
        cooldownOnChange = SkillConstants.instance.cooldownOnChange;
        cooldownSkill.x = cooldownSkill.y = SkillConstants.instance.medicSkillStat[3, levelSkill, 1];

        photonView = GetComponent<PhotonView>();
	}

    public virtual void Update()
    {
        if (photonView.isMine)
        {
            for (int I = 1; I < 5; I++)
            {
                // проверяем нажата ли кнопка переключения умения. 
                string _key = "selectSkill"+I.ToString();
                if (Input.GetButton(_key))
                    if (selectSkillKey == _key) // если клавиша нашего умения
                    {
                        if (!isSelected) // не включено ли умение уже?
                        {
                            print("SKILL SELECT 1"); // Если нет, то включаем и добавляем кулдаун штраф за смену умения
                            isSelected = true;
                            cooldownSkill.x = cooldownSkill.y + cooldownOnChange; 
                        }
                        break;
                    }
                    else
                    {
                        isSelected = false;
                        break;
                    }
            }
       
            if (isSelected)
            {
                if (cooldownSkill.x > 0) cooldownSkill.x-= Time.deltaTime; else
                {
                    if (Input.GetButton("skillActionKey") || isPassive) // Используем умение если нажата клавиша активации умения или если навык пассивен
                    {
                        if (UseSkill()) cooldownSkill.x = cooldownSkill.y;
                    }
                }
            }
        }
    }

    // Если умени применено успешно (например медик навел именно 
    // на дружественную цель и использовал лечение, а не на враге или в никуда)
    // то функция должна вернуть true. 
    public virtual bool UseSkill()
    {
        print("SKILL1");
        return true;
    }

    
}
