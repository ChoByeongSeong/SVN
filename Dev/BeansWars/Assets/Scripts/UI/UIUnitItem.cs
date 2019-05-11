using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIUnitItem : MonoBehaviour
{
    public Text txtCost;
    public Toggle toggle;

    public void Initialize(string unitName, int cost, MonoBehaviour owner)
    {
        txtCost.text = cost.ToString();
        toggle.group = owner.GetComponent<ToggleGroup>();

        toggle.onValueChanged.AddListener((on) =>
        {
            if (on)
            {
                ISetUnitName setUnitName = (ISetUnitName)owner;
                setUnitName.SetUnitName(unitName);
 
            }

            else
            {
                ISetUnitName setUnitName = (ISetUnitName)owner;
                setUnitName.SetUnitName(null);
            }
        });
    }

}
