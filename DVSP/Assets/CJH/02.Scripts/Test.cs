using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace DelegateTest
{
    class Test : MonoBehaviour
    {
        //반복문 카운트
        public InputField forCnt;

        class Move
        {
            public Vector3 Pos { get; set; }

            public Move(Vector3 pos)
            {
                Pos = pos;
            }
        }

        class ExeMove
        {
            private List<Move> listOfMove = new List<Move>();
            public delegate void MoveProcess(Move list);
            public int i = 0;

            public int listCnt()
            {
                return listOfMove.Count;
            }

            public Move ReturnMove(int index)
            {
                return listOfMove[index];
            }
            public void Add(Move move)
            {
                listOfMove.Add(move);
            }

            public void OnMove(MoveProcess process)
            {
                //무한 반복
                if (i == listOfMove.Count) return;
                //i %= listOfMove.Count;
                var item = listOfMove[i];
                process(item);
                i++;
            }
        }
        ExeMove moveExe;
        //함수 실행 여부
        bool exe = false;
        private void Start()
        {
            moveExe = new ExeMove();
        }

        float currtime = 0;
        float checktime = 1.5f;

        private void Update()
        {
            if (exe)
            {
                currtime += Time.deltaTime;
                if (currtime >= checktime)
                {
                    MoveExplorer();
                    currtime = 0;
                }
            }
            //좌측 이동
            if (Input.GetKeyDown(KeyCode.A))
            {
                moveExe.Add(new Move(Vector3.left));
            }
            //우측 이동
            else if (Input.GetKeyDown(KeyCode.D))
            {
                moveExe.Add(new Move(Vector3.right));
            }
            //상측 이동
            else if (Input.GetKeyDown(KeyCode.W))
            {
                moveExe.Add(new Move(Vector3.up));
            }
            //하측 이동
            else if (Input.GetKeyDown(KeyCode.S))
            {
                moveExe.Add(new Move(Vector3.down));
            }
            //함수 실행 키 Space bar
            else if (Input.GetKeyDown(KeyCode.X))
            {
                exe = !exe;
            }
        }

        void MoveExplorer()
        {
            moveExe.OnMove((move) =>
                {
                    transform.position += move.Pos;
                });
        }
        bool clickfor = true;
        int index;
        //반복문 담기
        public void OnClickFor()
        {
            //첫 번째 클릭
            if (clickfor)
            {
                //담기 시작할 때의 인덱스
                index = moveExe.listCnt();
                clickfor = false;
                print("첫 번째 클릭  ,  " + index);
            }
            //두 번째 클릭
            else
            {
                print("두 번째 클릭  , " + moveExe.listCnt());
                //반복문 지정 인덱스만큼 돌리기

                for (int i = 0; i < 3; i++)
                {
                    //반복문 시작 때 담은 함수의 인덱스부터 끝날 때 담은 인덱스까지 리스트에 담기 
                    for (int j = index; j < moveExe.listCnt(); j++)
                    {
                        //print(moveExe.ReturnMove(j));
                        moveExe.Add(moveExe.ReturnMove(j));
                    }
                }
                clickfor = true;
            }
        }
    }
}
