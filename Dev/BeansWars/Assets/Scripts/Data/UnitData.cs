using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("YellowBean/Data/UnitData"))]
public class UnitData : ScriptableObject
{
    // 생성자
    public UnitData() { }

    // 스텟
    public int id;
    public new string name;
    public string thum_name;
    public string display_name;
    public float hp;
    public float attack_damage;
    public float attack_speed;
    public float attack_range;
    public float armor;
    public float move_speed;
    public float mass;
    public int cost;
    public int type;
    public int target_priority;

    public UnitData(UnitData other)
    {
        this.id = other.id;
        this.name = other.name;
        this.thum_name = other.thum_name;
        this.display_name = other.display_name;
        this.hp = other.hp;
        this.attack_damage = other.attack_damage;
        this.attack_speed = other.attack_speed;
        this.attack_range = other.attack_range;
        this.armor = other.armor;
        this.move_speed = other.move_speed;
        this.mass = other.mass;
        this.cost = other.cost;
        this.type = other.type;
        this.target_priority = other.target_priority;
    }
}
