using UnityEngine;
using UnityEditor;
using System.Collections;

public class Magic:Projectile
{
    float speed = 10f;
    Unit target;

    public void Initialize(Unit owner, Unit target, Vector3 startPos, Vector3 endPos)
    {
        base.owner = owner;
        tag = owner.tag;

        this.target = target;

        base.startPos = startPos;
        base.endPos = endPos;

        transform.position = startPos;

        StopCoroutine("Shoot");
        StartCoroutine("Shoot");
    }

    IEnumerator Shoot()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);

            float dst = (transform.position - endPos).sqrMagnitude;
            if(dst < 0.1f)
            {
                break;
            }

            yield return null;
        }
        
        if(target.alive)
        {
            Attacker attacker = new Attacker
            {
                damage = owner.status.attack_damage
            };

            target.Hit(attacker);
        }

        ProjectilesPool.instance.DestroyMagic(this);
    }
}