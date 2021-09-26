using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Vector3Int currentPlayerChunkPosition;
    public World world;

    public float detectionInterval = 1f;
    public CinemachineVirtualCamera cam;

    private Vector3Int currentChunkCenter = Vector3Int.zero;
    private GameObject player;

    public void SpawnPlayer()
    {
        if (player != null)
            return;
        
        Vector3Int raycastStartPosition = new Vector3Int(world.chunkSize / 2, 100, world.chunkSize / 2);
        RaycastHit hit;
        if (Physics.Raycast(raycastStartPosition, Vector3.down, out hit, 120))
        {
            player = Instantiate(playerPrefab, hit.point + Vector3Int.up, Quaternion.identity);
            StartCheckingTheMap();
            cam.Follow = player.transform.GetChild(2);
        }
    }

    public void StartCheckingTheMap()
    {
        SetCurrentChunkCoords();
        StopAllCoroutines();
        StartCoroutine(LoadNextPosition());
    }

    private IEnumerator LoadNextPosition()
    {
        yield return new WaitForSeconds(detectionInterval);
        if (Mathf.Abs(currentChunkCenter.x - player.transform.position.x) > world.chunkSize ||
            Mathf.Abs(currentChunkCenter.z - player.transform.position.z) > world.chunkSize ||
            Mathf.Abs(currentPlayerChunkPosition.y - player.transform.position.y) > world.chunkHeight)
        {
            world.LoadAdditionalChunksRequest(player);
        }
        else StartCoroutine(LoadNextPosition());
    }

    private void SetCurrentChunkCoords()
    {
        currentPlayerChunkPosition = WorldDataHelper.ChunkPositionFromBlockCoords(world, Vector3Int.RoundToInt(player.transform.position));
        currentChunkCenter.x = currentPlayerChunkPosition.x + world.chunkSize / 2;
        currentChunkCenter.z = currentPlayerChunkPosition.z + world.chunkSize / 2;
    }
}
