using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlayerAction
{
    public float actionTime { get; private set; }
    public int actionEnergyCost { get; private set; }

    public ARPlayerAction()
    {
        actionTime = 0f;
        actionEnergyCost = 1;
    }

    public ARPlayerAction AttackAction()
    {
        actionTime = 2f;
        actionEnergyCost = 10;
        return this;
    }

    public ARPlayerAction DialogueAction()
    {
        actionTime = 5f;
        actionEnergyCost = 2;
        return this;
    }

    public ARPlayerAction InspectAction()
    {
        actionTime = 3f;
        actionEnergyCost = 2;
        return this;
    }

    public ARPlayerAction LootAction()
    {
        actionTime = 2f;
        actionEnergyCost = 5;
        return this;
    }

    public ARPlayerAction WalkAction()
    {
        actionTime = 5f;
        actionEnergyCost = 10;
        return this;
    }


}
