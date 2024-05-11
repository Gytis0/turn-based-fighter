using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UtilityTests
{
    [Test]
    public void GetFacingDirection_Test()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        Assert.AreEqual(Direction.Forward, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 90, 0);
        Assert.AreEqual(Direction.Right, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 180, 0);
        Assert.AreEqual(Direction.Backward, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 270, 0);
        Assert.AreEqual(Direction.Left, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 359, 0);
        Assert.AreEqual(Direction.Forward, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 0, 0);
        Assert.AreEqual(Direction.Forward, UtilityScripts.GetFacingDirection(rotation));

        rotation = Quaternion.Euler(0, 360, 0);
        Assert.AreEqual(Direction.Forward, UtilityScripts.GetFacingDirection(rotation));
    }
}
