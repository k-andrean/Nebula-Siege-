/*
 * Copyright (c) 2021 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System.Runtime.Versioning;
using UnityEngine;

//namespace RayWenderlich.SpaceInvadersUnity
namespace KelvinAndrean.NebulaSiege
{
    public class InvaderSwarm : MonoBehaviour
    {
        [System.Serializable]
        private struct InvaderType
        {
            public string name;
            public Sprite[] sprites;
            public int points;
            public int rowCount;
            public bool isSpecialColor;
        }

        internal static InvaderSwarm Instance;

        [Header("Spawning")]
        [SerializeField]
        private InvaderType[] invaderTypes;

        [SerializeField]
        private int columnCount = 11;

        [SerializeField]
        private int ySpacing;

        [SerializeField]
        private int xSpacing;

        [SerializeField]
        private Transform spawnStartPoint;

        private float minX;

        [Space]
        [Header("Movement")]
        [SerializeField]
        private float speedFactor = 10f;

        private Transform[,] invaders;
        private int rowCount;
        private bool isMovingRight = true;
        private float maxX;
        private float currentX;
        private float xIncrement;

        private int killCount;
        private System.Collections.Generic.Dictionary<string, int> pointsMap;

        [SerializeField]
        private MusicControl musicControl;

        private int tempKillCount;

        [SerializeField]
        private Transform cannonPosition;

        private float minY;
        private float currentY;

        [Header("Special Invader Settings")]
        [SerializeField] private float specialInvaderChance = 0.2f; // 20% chance for each invader to be special
        [SerializeField] private Color specialInvaderColor = Color.yellow;

        internal void IncreaseDeathCount()
        {
            killCount++;
            if (killCount >= invaders.Length)
            {
                GameManager.Instance.TriggerGameOver(false);
                return;
            }

            tempKillCount++;
            if (tempKillCount < invaders.Length / musicControl.pitchChangeSteps)
            {
                return;
            }

            musicControl.IncreasePitch();
            tempKillCount = 0;

        }

        internal int GetPoints(string alienName)
        {
            if (pointsMap.ContainsKey(alienName))
            {
                return pointsMap[alienName];
            }
            return 0;
        }


        internal Transform GetInvader(int row, int column)
        {
            if (row < 0 || column < 0
                || row >= invaders.GetLength(0) || column >= invaders.GetLength(1))
            {
                return null;
            }

            return invaders[row, column];
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            if (GameSettings.Instance != null)
            {
                int totalRows = GameSettings.Instance.GetRowCount();

                for (int i = 0; i < invaderTypes.Length; i++)
                {
                    invaderTypes[i].rowCount = totalRows;
                }
            }
        }


        private void Start()
        {
            minX = spawnStartPoint.position.x;

            currentY = spawnStartPoint.position.y;
            minY = cannonPosition.position.y;

            GameObject swarm = new GameObject { name = "Swarm" };
            Vector2 currentPos = spawnStartPoint.position;

            foreach (var invaderType in invaderTypes)
            {
                rowCount += invaderType.rowCount;
            }
            maxX = minX + 2f * xSpacing * columnCount;
            currentX = minX;
            invaders = new Transform[rowCount, columnCount];

            pointsMap = new System.Collections.Generic.Dictionary<string, int>();

            int rowIndex = 0;
            foreach (var invaderType in invaderTypes)
            {
                var invaderName = invaderType.name.Trim();
                pointsMap[invaderName] = invaderType.points;
                for (int i = 0, len = invaderType.rowCount; i < len; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        var invader = new GameObject() { name = invaderName };
                        invader.AddComponent<SimpleAnimator>().sprites = invaderType.sprites;
                        invader.transform.position = currentPos;
                        invader.transform.SetParent(swarm.transform);
                        invaders[rowIndex, j] = invader.transform;

                        if (Random.value < specialInvaderChance)
                        {
                            var otherColorInvader = invader.AddComponent<OtherColorInvader>();
                            // otherColorInvader.powerUpType = (OtherColorInvader.PowerUpType)Random.Range(0, 3);
                            if (rowIndex == 0)
                            {
                                otherColorInvader.powerUpType = OtherColorInvader.PowerUpType.MoveSpeed;
                            }
                            else
                            {
                                otherColorInvader.powerUpType = (OtherColorInvader.PowerUpType)Random.Range(0, 2);
                            }
                        }
                        //  if (i == 0)
                        // {
                        //     var otherColorInvader = invader.AddComponent<OtherColorInvader>();
                        //     otherColorInvader.powerUpType = OtherColorInvader.PowerUpType.Invincibility;
                        // }
                        // // For other rows, use the random chance
                        // else if (Random.value < specialInvaderChance)
                        // {
                        //     var otherColorInvader = invader.AddComponent<OtherColorInvader>();
                        //     otherColorInvader.powerUpType = (OtherColorInvader.PowerUpType)Random.Range(0, 3);
                        // }


                        currentPos.x += xSpacing;
                    }

                    currentPos.x = minX;
                    currentPos.y -= ySpacing;

                    rowIndex++;
                }
            }

            for (int i = 0; i < columnCount; i++)
            {

                var bulletSpawnerObject = Instantiate(Resources.Load("Prefabs/BulletSpawner")) as GameObject;
                var bulletSpawner = bulletSpawnerObject.GetComponent<BulletSpawner>();

                bulletSpawnerObject.transform.SetParent(swarm.transform);
                bulletSpawner.column = i;
                bulletSpawner.currentRow = rowCount - 1;
                bulletSpawner.Setup();

            }

        }

        private void Update()
        {
            xIncrement = speedFactor * musicControl.Tempo * Time.deltaTime;
            if (isMovingRight)
            {
                currentX += xIncrement;
                if (currentX < maxX)
                {
                    MoveInvaders(xIncrement, 0);
                }
                else
                {
                    ChangeDirection();
                }
            }
            else
            {
                currentX -= xIncrement;
                if (currentX > minX)
                {
                    MoveInvaders(-xIncrement, 0);
                }
                else
                {
                    ChangeDirection();
                }
            }
        }

        private void MoveInvaders(float x, float y)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    invaders[i, j].Translate(x, y, 0);
                }
            }
        }

        private void ChangeDirection()
        {
            isMovingRight = !isMovingRight;
            MoveInvaders(0, -ySpacing);

            currentY -= ySpacing;
            if (currentY < minY)
            {
                GameManager.Instance.TriggerGameOver();
            }

        }


    }
}