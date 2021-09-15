using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ET;
using DG.Tweening;

namespace EGamePlay
{
    public interface IPosition
    {
        Vector3 Position { get; set; }
    }
}