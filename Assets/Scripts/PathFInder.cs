﻿using System;
using System.Collections.Generic;
// using Rewired;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace services
{
    public class PathFinder
    {
        public class Path
        {
            public int g;         // Steps from A to this
            public int h;         // Steps from this to B
            public Path parent;   // Parent node in the path
            public int x;         // x coordinate
            public int y;         // y coordinate
            public int p;

            public Path (int _g, int _h, Path _parent, int _x, int _y)
            {
                g = _g;
                h = _h;
                parent = _parent;
                x = _x;
                y = _y;
            }
         
            public int f // Total score for this
            {
                get 
                {
                    return g+h+p; 
                }
            }
        }

        private bool fancyDrawDebug = false;
        private List<Vector2Int> _failCache = new List<Vector2Int>();
        private iNavagatable searcher;
        private GameObject target;
        private List<Path> _cacehdPaths;
        private Vector2Int _cachePos;
        private bool isNavigating = false;
        private const float OFFSET = .5f;

        public PathFinder(iNavagatable searcher, GameObject target)
        {
            this.searcher = searcher;
            this.target = target;
        }
        
        private List<Path> GetAdjacentSquares(Path p) {
            List<Path> ret = new List<Path> ();
            int _x = p.x;
            int _y = p.y;
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++)
                {
                    int __x = _x + x; // easier than writing (_x + x) 5 times
                    int __y = _y + y; // easier than writing (_y + y) 5 times
                    // skip self and diagonal squares
                    if ((x == 0 && y == 0) /*|| (x != 0 && y != 0)*/)
                    {
                    

                        continue;
                    }else if (
                        !CheckForCollision(p, new Vector2(__x + .5f, __y + .5f))
                    )
                    {

                        Path newPath = new Path(p.g + 1,
                            BlocksToTarget(new Vector2(__x, __y), target.transform.position),
                            p, __x, __y);
                       
                        ret.Add(newPath);
                       
                    }
                }
            }
            return ret;
        }

        private int BlocksToTarget(Vector2 vector2, Vector2 position)
        {
            return (int) Math.Ceiling(Math.Abs((position - vector2).sqrMagnitude));
        }

        private bool CheckForCollision(Path p, Vector2 end) {
            
            Vector2 startPos = new Vector2(p.x + OFFSET, p.y + OFFSET);
            searcher.GetComponent<PolygonCollider2D>().enabled = false;
            PolygonCollider2D targetCollider = target.GetComponent<PolygonCollider2D>();
            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }

            Vector2 direction = (end - startPos);
           
            float radius = .5f;
            RaycastHit2D hit = Physics2D.CircleCast(startPos,  radius, direction, direction.sqrMagnitude);//, GameManager.instance.PrefabManager.defaultParent.gameObject.layer);

            
            searcher.GetComponent<PolygonCollider2D>().enabled = true;
            if (targetCollider != null)
            {
                targetCollider.enabled = true;
            }
            // trying to walk into a wall, change direction
         
            /*if (
                hit.transform != null &&
                (
                    hit.collider.GetComponent<MouseObject>() == null && //TODO: Fix this super hackyness
                    hit.collider.GetComponent<VisibilityObject>() == null
                )
            )
            {
                
               Debug.Log("PathFinderHit: " + hit.collider.name);
                Debug.DrawLine(startPos, end, Color.red);
                return true;
            }*/

            Debug.DrawLine(startPos, end, Color.blue);
           
            return false;
        }

        public List<Path> FindPath()
        {
            fancyDrawDebug = true;
            if (_cacehdPaths != null)
            {
                
                if (
                    !Math.Ceiling(target.transform.position.x).Equals(_cachePos.x) ||
                    !Math.Ceiling(target.transform.position.y).Equals(_cachePos.y)
                )
                {

                    _cacehdPaths = null;
                    _cachePos = new Vector2Int((int)Math.Ceiling(target.transform.position.x), (int)Math.Ceiling(target.transform.position.y));
                }
                else
                {
                    return _cacehdPaths;
                }

            }
    
           
            Path destPath = null;
            List<Path> closedPathList = new List<Path>();
            List<Path> openPathList = new List<Path>();
            Path destinationSquare = new Path (
                0, 
                0, 
                null,
                (int)target.transform.position.x, 
                (int)target.transform.position.y
            );
            
            openPathList.Add(
                new Path(
                    0, 
                    BlocksToTarget(
                        searcher.transform.position, 
                        target.transform.position
                    ), 
                    null,
                    (int)searcher.transform.position.x, 
                    (int)searcher.transform.position.y
                )
            );
            Path currentSquare;
            int saftyCount = 0;
            const int SAFTY_MAX = 256;
            while (
                openPathList.Count > 0
            )
            {
                saftyCount += 1;
                if (saftyCount >= SAFTY_MAX)
                {
                    //Debug.LogError("We reached the saftyCatch. Open Path Count: "  +openPathList.Count + " " + closedPathList.Count);
                    
                    return null;
                    //throw new System.Exception("We reached the saftyCatch. Open Path Count: "  +openPathList.Count + " " + closedPathList.Count);
                }
                openPathList.Sort(delegate(Path p1, Path p2) { return p1.f.CompareTo(p2.f); });
                currentSquare = openPathList[0];
                closedPathList.Add(currentSquare);
                openPathList.Remove(currentSquare);   
                // The target has been located
                foreach (Path path in closedPathList)
                {
                    if (
                        path.x.Equals(destinationSquare.x) &&
                        path.y.Equals(destinationSquare.y)
                    )
                    {
                        _cacehdPaths = buildPath(currentSquare);
                        return _cacehdPaths;

                    }
                }
               

                List<Path> adjacentSquares = GetAdjacentSquares(currentSquare);
                if (adjacentSquares.Count < 8)
                {
                    currentSquare.g += 50;
                }
                foreach (Path p in adjacentSquares)
                {
                    if (closedPathList.Contains(p))
                    {
                        continue; // skip this one, we already know about it
                    }

                    if (!openPathList.Contains(p))
                    {
                        openPathList.Add(p);
                    }
                    else if (p.h + currentSquare.g + 1 < p.f)
                    {
                        p.parent = currentSquare;
                    }
                }

               
            }

           
            return null;
        }
        
        private List<Path> buildPath(Path p) {
            List<Path> bestPath = new List<Path> ();
            Path currentLoc = p;
            Path lastMove = null;
            bestPath.Insert (0,currentLoc);
            while (currentLoc.parent != null) {
                currentLoc = currentLoc.parent;
                if (currentLoc.parent != null)
                {
                    bestPath.Insert(0, currentLoc);
                }
                else
                {

                    lastMove = currentLoc;
                }
            }
            return bestPath;
        }

        public void SetTarget(GameObject gameObject)
        {
            if (gameObject.Equals((target)))
            {
                return; 
            }
            _cacehdPaths = null;
            target = gameObject;
            _cachePos = new Vector2Int((int)Math.Ceiling(target.transform.position.x), (int)Math.Ceiling(target.transform.position.y));
        }

        public void tickNavigate()
        {
            if (fancyDrawDebug)
            {
                fancyDrawDebug = false;
            }
            if (!isNavigating)
            {
                return;
            }
            
        
            List<PathFinder.Path> pathTo = FindPath();
            if (pathTo == null)
            {
                Debug.LogError("Cannot find dest");
                isNavigating = false;
                return;
            }
          
            
            

            if (pathTo.Count == 0)
            {
                //TODO: Add an event?
                isNavigating = false;
                return;
            }
            
            Path nextMove = pathTo[0];
            //Debug.Log("Executing NPCMoveToTask: " + nextMove.x + ", " + nextMove.y + " == " +  Math.Round(searcher.transform.position.x) + ", "+  Math.Round(searcher.transform.position.y)+ "    ----   " +  pathTo.Count);
            if (
                Math.Ceiling(searcher.transform.position.x).Equals(nextMove.x + 1) &&
                Math.Ceiling(searcher.transform.position.y).Equals(nextMove.y + 1)
            )
            {
                pathTo.RemoveAt (0);
            }

            Vector3 lastPos = searcher.transform.position;
            foreach (Path path in pathTo)
            {
                Vector3 nextPos = new Vector3(path.x + OFFSET, path.y + OFFSET);
                
                Debug.DrawLine( nextPos + new Vector3(.1f, .1f),  lastPos+ new Vector3(.1f, .1f));
                lastPos = nextPos;
            }

            Rigidbody2D rigidbody2D = searcher.GetComponent<Rigidbody2D>(); //searcher.GetRigidbody2D(); 
            Vector2 directionTo = (new Vector2(nextMove.x + OFFSET, nextMove.y + OFFSET) - (Vector2)searcher.transform.position).normalized ;
            float speed = searcher.speed;
           
            rigidbody2D.velocity = directionTo* speed;

            rigidbody2D.rotation = ChaosUtil.AngleBetweenVector2(Vector2.zero, directionTo) + 90;
        }

        public bool navigateTo(GameObject target)
        {
            Vector2Int targetPos = new Vector2Int(
                (int) target.transform.position.x,
                (int) target.transform.position.y
            );
            if (_failCache.Contains(targetPos))
            {
                return false;
            }
            SetTarget(target);
            List<Path> paths = FindPath();
            if (paths == null)
            {
                _failCache.Add(targetPos);
                return false;
            }
            isNavigating = true;
            return true;
        }

        public void stopNaviagting()
        {
            isNavigating = false;
        }

        public void clearFailCache()
        {
            _failCache.Clear();
        }
    }
    
}