using System.Collections.Generic;
using UnityEngine;

namespace XNClient.ChessBoard
{
    public class RectGrid : MonoBehaviour
    {
        public GameObject chunkPrefab;
        public List<RectGridChunk> chunkList = new List<RectGridChunk>();
    }
}
