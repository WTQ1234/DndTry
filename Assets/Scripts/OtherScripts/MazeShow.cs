using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeShow : MonoBehaviour
{
    public MazeCreate mazeCreate;
    public GameObject maze;
    float addTime = 0;
    int addindex = 0;

    void Update()
    {
        if (addindex >= mazeCreate.findList.Count)
        {
            return;
        }

        addTime += Time.deltaTime;

        if (addTime > 0.05)
        {
            addTime = 0;
            int index = mazeCreate.findList[addindex];

            int _row = index / mazeCreate.row;
            int _col = index % mazeCreate.col;

            GameObject column = maze;
            column = MonoBehaviour.Instantiate(column);
            column.transform.position = new Vector3(_row, 0, _col);

            addindex++;
        }
    }
}
