using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealable
{
    void OnHealed(float _fMinHp, float _fMaxHp);
}
