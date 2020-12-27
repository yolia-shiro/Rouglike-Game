using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCreate : MonoBehaviour
{
    public enum walls { U = 1, L, UL, D, UD, DL, UDL, R, UR, LR, ULR, DR, UDR, DLR, UDLR };

    public int roomNums;
    public float xOffset;
    public float yOffset;
    public GameObject roomPrefabs;
    public float checkRadius;
    public LayerMask roomMask;

    public WallType wallType;

    private Room[] rooms;
    private Room[,] edges;
    private Vector3[] posOffset;

    private void Awake()
    {
        CreateRooms();
        CreateDoor();
        CreateWall();
    }

    private void CreateRooms()
    {
        rooms = new Room[roomNums];
        edges = new Room[roomNums, 4];
        posOffset = new Vector3[4] { new Vector3(0, yOffset, 0), new Vector3(-xOffset, 0, 0), new Vector3(0, -yOffset, 0), new Vector3(xOffset, 0, 0) };
        int curNum = 0;
        int curIndex = 0;
        Vector3 curPos = new Vector3(0, 0, 0);
        if (roomNums > 0)
        {
            rooms[0] = Instantiate(roomPrefabs, curPos, Quaternion.identity).GetComponent<Room>();
            rooms[0].SetValue(curNum, curPos);
        }

        int dir;
        Collider2D check;

        while (curNum < roomNums - 1)
        {
            do
            {
                dir = Random.Range(0, 4);
            } while (edges[curIndex, dir] != null);
            // check && edges[check.GetComponent<Room>().roomIndex, (dir + 2) % 4] != null 的情况是不会存在的。
            // edges[check.GetComponent<Room>().roomIndex, (dir + 2) % 4] != null 则 check 是不可能检测到当前物体的。
            check = Physics2D.OverlapCircle(curPos + posOffset[dir], checkRadius, roomMask);
            if (check && edges[check.GetComponent<Room>().roomIndex, (dir + 2) % 4] == null)
            {
                //连边
                edges[curIndex, dir] = rooms[check.GetComponent<Room>().roomIndex];
                edges[check.GetComponent<Room>().roomIndex, (dir + 2) % 4] = rooms[curIndex];
                curIndex = check.GetComponent<Room>().roomIndex;
                curPos += posOffset[dir];
                continue;
            }

            if (curNum < roomNums - 2)
            {
                edges[curIndex, dir] = Instantiate(roomPrefabs, curPos + posOffset[dir], Quaternion.identity).GetComponent<Room>();
                rooms[curNum + 1] = edges[curIndex, dir];
                rooms[curNum + 1].SetValue(curNum + 1, curPos + posOffset[dir]);
                edges[curNum + 1, (dir + 2) % 4] = rooms[curIndex];
            }
            curNum++;
            curIndex = curNum;
            curPos += posOffset[dir];
        }

        //寻找最远房间，并对每个房间计算距离
        bool[] visited = new bool[roomNums];
        Queue<int> q = new Queue<int>();
        List<int> res = new List<int>();
        int height = 0;
        q.Enqueue(0);
        res.Add(0);
        while (q.Count > 0)
        {
            int size = q.Count;
            res.Clear();
            for (int i = 0; i < size; i++)
            {
                int index = q.Dequeue();
                res.Add(index);
                visited[index] = true;
                rooms[index].dis = height;
                rooms[index].indexText.text = height.ToString();
                for (int j = 0; j < 4; j++)
                {
                    if (edges[index, j] != null && !visited[edges[index, j].roomIndex])
                    {
                        //添加到队列，并且显示门
                        q.Enqueue(edges[index, j].roomIndex);
                        //rooms[index].transform.GetChild(j).gameObject.SetActive(true);
                    }
                }
            }
            height++;
        }
        //生成最后一个房间(只有一个出口)
        //在最远的房间中随机选择一个
        int farRoomIndex = res[Random.Range(0, res.Count)];
        //存储最后一个房间中剩余能够放置门的位置
        List<int> finalDoorPos = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            if (edges[farRoomIndex, i] == null &&
                !Physics2D.OverlapCircle(rooms[farRoomIndex].position + posOffset[i], checkRadius, roomMask))
            {
                finalDoorPos.Add(i);
            }
        }
        int randomDir = finalDoorPos[Random.Range(0, finalDoorPos.Count)];
        //生成最后一个房间
        edges[farRoomIndex, randomDir] = Instantiate(roomPrefabs,
                                                            rooms[farRoomIndex].position + posOffset[randomDir],
                                                            Quaternion.identity).GetComponent<Room>();
        rooms[roomNums - 1] = edges[farRoomIndex, randomDir];
        rooms[roomNums - 1].SetValue(roomNums - 1, rooms[farRoomIndex].position + posOffset[randomDir], rooms[farRoomIndex].dis + 1);
        edges[roomNums - 1, (randomDir + 2) % 4] = rooms[farRoomIndex];
    }

    private void CreateDoor()
    {
        //生成门
        for (int i = 0; i < roomNums; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (edges[i, j] != null)
                {
                    rooms[i].transform.GetChild(j).gameObject.SetActive(true);
                    rooms[i].doorPos[j] = true;
                }
            }
        }
    }

    private void CreateWall()
    {
        //生成墙壁
        for (int i = 0; i < roomNums; i++) 
        {
            int[] arr = new int[1];
            rooms[i].doorPos.CopyTo(arr, 0);
            switch (arr[0])
            {
                case (int)walls.R:
                    Instantiate(wallType.wall_R, rooms[i].transform);
                    break;
                case (int)walls.D:
                    Instantiate(wallType.wall_D, rooms[i].transform);
                    break;
                case (int)walls.DR:
                    Instantiate(wallType.wall_DR, rooms[i].transform);
                    break;
                case (int)walls.L:
                    Instantiate(wallType.wall_L, rooms[i].transform);
                    break;
                case (int)walls.LR:
                    Instantiate(wallType.wall_LR, rooms[i].transform);
                    break;
                case (int)walls.DL:
                    Instantiate(wallType.wall_DL, rooms[i].transform);
                    break;
                case (int)walls.DLR:
                    Instantiate(wallType.wall_DLR, rooms[i].transform);
                    break;
                case (int)walls.U:
                    Instantiate(wallType.wall_U, rooms[i].transform);
                    break;
                case (int)walls.UR:
                    Instantiate(wallType.wall_UR, rooms[i].transform);
                    break;
                case (int)walls.UD:
                    Instantiate(wallType.wall_UD, rooms[i].transform);
                    break;
                case (int)walls.UDR:
                    Instantiate(wallType.wall_UDR, rooms[i].transform);
                    break;
                case (int)walls.UL:
                    Instantiate(wallType.wall_UL, rooms[i].transform);
                    break;
                case (int)walls.ULR:
                    Instantiate(wallType.wall_ULR, rooms[i].transform);
                    break;
                case (int)walls.UDL:
                    Instantiate(wallType.wall_UDL, rooms[i].transform);
                    break;
                case (int)walls.UDLR:
                    Instantiate(wallType.wall_UDLR, rooms[i].transform);
                    break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, checkRadius);
    }
}
[System.Serializable]
public class WallType 
{
    public GameObject wall_U, wall_L, wall_UL, wall_D
        , wall_UD, wall_DL, wall_UDL, wall_R
        , wall_UR, wall_LR, wall_ULR, wall_DR
        , wall_UDR, wall_DLR, wall_UDLR;
};
