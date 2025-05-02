using System.Collections.Generic;
using UnityEngine;

public class CheckpointTeleportManager : MonoBehaviour
{
    public Transform transformToMovePlayer;
    public GameObject firstPlayerPos;


    private void Start()
    {
        transformToMovePlayer = firstPlayerPos.transform;
    }

    public void CheckLastCheckpointAchieved(Transform checkpointPos)
    {
        transformToMovePlayer = checkpointPos;
    }
}
