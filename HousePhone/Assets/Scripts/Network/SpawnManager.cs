using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class SpawnManager : MonoBehaviour
{
    PhotonView pv;
    void Awake()
    {
        pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            if (pv.Owner.IsMasterClient)
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HostManager"), Vector3.zero, Quaternion.identity);
            }
            else
            {
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ClientManager"), Vector3.zero, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
