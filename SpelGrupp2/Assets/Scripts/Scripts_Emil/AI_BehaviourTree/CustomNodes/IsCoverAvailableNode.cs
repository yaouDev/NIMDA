using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehavior/Behavior/IsCoverAvailable")]

public class IsCoverAvailableNode : Node
{
    private Transform getBestSpot;
    //private Transform[] availableCoverSpots;
    private Cover[] availableCovers;
    private Transform target;


    public IsCoverAvailableNode(Cover[] availableCovers, Transform target)
    {
        this.availableCovers = availableCovers;
        this.target = target;
    }

    public override NodeState Evaluate()
    {
        availableCovers = AIData.instance.GetActiveCovers();
        Transform bestSpot = FindBestCoverSpot();
        AIData.instance.SetBestCoverSpot(bestSpot);
        return bestSpot != null ? NodeState.SUCCESS : NodeState.FAILURE;
    }

    private Transform FindBestCoverSpot()
    {
        getBestSpot = AIData.instance.GetBestCoverSpot();

        if (getBestSpot != null)
        {
            if (CheckIfSpotIsValid(getBestSpot))
            {
                return getBestSpot;
            }
        }

        float minAngle = 90;
        Transform bestSpot = null;
        availableCovers = AIData.instance.GetActiveCovers();

        for (int i = 0; i < availableCovers.Length; i++)
        {
            Transform bestSpotInCover = FindBestSpotInCover(availableCovers[i], ref minAngle);
            if (bestSpotInCover != null)
            {
                bestSpot = bestSpotInCover;
            }
        }
        return bestSpot;
    }

    private Transform FindBestSpotInCover(Cover cover, ref float minAngle)
    {
        Transform[] availableSpots = cover.GetCoverSpots();
        Transform bestSpot = null;

        for (int i = 0; i < availableSpots.Length; i++)
        {
            Vector3 direction = agent.ClosestPlayer - availableSpots[i].position;
            if (CheckIfSpotIsValid(availableSpots[i]))
            {
                float angle = Vector3.Angle(availableSpots[i].forward, direction);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    bestSpot = availableSpots[i];
                }
            }
        }
        return bestSpot;
    }

    private bool CheckIfSpotIsValid(Transform spot)
    {
        RaycastHit hit;
        Vector3 direction = agent.ClosestPlayer - spot.position;
        if (Physics.Raycast(spot.position, direction, out hit))
        {
            if (hit.collider.transform != agent)
            {
                return true;
            }

        }
        return false;
    }
}
