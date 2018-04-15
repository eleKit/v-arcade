using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;

public class PathSaveData : MonoBehaviour
{

	public static PathSaveData pathData;
	

	List<PathInfos> paths = new List<PathInfos>();
	PathInfos currentPath;
	int obstacleInterval = 3;
	int targetPoints = 20;

	void Awake()
	{
		if (pathData == null)
		{
			DontDestroyOnLoad(gameObject);
			pathData = this;
			LoadPaths();
		}
		else if (pathData != this)
		{
			Destroy(gameObject);
		}
	}

	public bool CheckPathName(string pName)
	{
		return paths.Exists(x => x.pathName.Equals(pName));
	}

	public void InitializePath()
	{
		currentPath = new PathInfos();
		Debug.Log("Percorso Inizializzato");
	}

	public void SetCurrentPathName(string pName)
	{
		currentPath.pathName = pName;
	}

	public void AddObstacle(string oName, Vector3 pos, Vector3 rot)
	{
		ObstacleInfos obstacle = new ObstacleInfos();
		obstacle.obstacleName = oName;
		obstacle.xPos = pos.x;
		obstacle.yPos = pos.y;
		obstacle.zPos = pos.z;
		obstacle.xAngle = rot.x;
		obstacle.yAngle = rot.y;
		obstacle.zAngle = rot.z;
		currentPath.obstacles.Add(obstacle);
	}

	public void SavePaths()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/paths.dat");
		if (currentPath != null)
		{
			paths.Add(currentPath);
			bf.Serialize(file, paths);
			file.Close();
			LoadPaths();
		}
		else
		{
			bf.Serialize(file, paths);
			file.Close();
			LoadPaths();
		}
	}

	public void LoadPaths()
	{
		if (File.Exists(Application.persistentDataPath + "/paths.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/paths.dat", FileMode.Open);
			paths = (List<PathInfos>)bf.Deserialize(file);
			file.Close();
		}
	}

	public List<string> GetPathNames()
	{
		List<string> names = new List<string>();
		for (int i = 0; i < paths.Count; i++)
		{
			names.Add(paths[i].pathName);
		}
		return names;
	}

	public List<Vector3> GetPathPositions(string pName)
	{
		List<Vector3> positions = new List<Vector3>();
		if (paths.Exists(x => x.pathName.Equals(pName)))
		{
			PathInfos path = paths.Find(x => x.pathName.Equals(pName));
			for (int i = 0; i < path.obstacles.Count; i++)
			{
				Vector3 position = new Vector3(path.obstacles[i].xPos, path.obstacles[i].yPos, path.obstacles[i].zPos);
				positions.Add(position);
			}
		}
		return positions;
	}

	public List<Vector3> GetPathAngles(string pName)
	{
		List<Vector3> angles = new List<Vector3>();
		if (paths.Exists(x => x.pathName.Equals(pName)))
		{
			PathInfos path = paths.Find(x => x.pathName.Equals(pName));
			for (int i = 0; i < path.obstacles.Count; i++)
			{
				Vector3 angle = new Vector3(path.obstacles[i].xAngle, path.obstacles[i].yAngle, path.obstacles[i].zAngle);
				angles.Add(angle);
			}
		}
		return angles;
	}

	public void DeletePath(string pathName)
	{
		if (paths.Exists(x => x.pathName.Equals(pathName)))
			paths.Remove(paths.Find(x => x.pathName.Equals(pathName)));
		SavePaths();
	}

	public int GetObstacleInterval()
	{
		return obstacleInterval;
	}

	public void SetObstacleInterval(int ob)
	{
		obstacleInterval = ob;
	}


	public void UpdateHighscore(string path, string player, int score)
	{
		if (paths.Exists(x => x.pathName.Equals(path)))
		{
			if (score > paths.Find(x => x.pathName.Equals(path)).highscore)
			{
				paths.Find(x => x.pathName.Equals(path)).highscore = score;
				paths.Find(x => x.pathName.Equals(path)).bestPlayer = player;
			}
		}
	}

	public string GetBestPlayer(string pathName)
	{
		string pName = "";
		if (paths.Exists(x => x.pathName.Equals(pathName)))
		{
			pName = paths.Find(x => x.pathName.Equals(pathName)).bestPlayer;
		}
		return pName;
	}

	public int GetPathHighscore(string pathName)
	{
		int high = 0;
		if (paths.Exists(x => x.pathName.Equals(pathName)))
		{
			high = paths.Find(x => x.pathName.Equals(pathName)).highscore;
		}
		return high;
	}

	public int GetTargetPoints()
	{
		return targetPoints;
	}
}


// Information about a game level
[Serializable]
class PathInfos
{
	public string pathName;
	public List<ObstacleInfos> obstacles;

	// Highest score achieved in this path
	public int highscore;

	// Name of the player who achieved the best score in this path
	public string bestPlayer;

	public PathInfos()
	{
		obstacles = new List<ObstacleInfos>();
		highscore = 0;
		bestPlayer = "";
	}
}

// Position and angle for a 3D obstacle (Plane game)
[Serializable]
class ObstacleInfos
{
	public string obstacleName;
	public float xPos;
	public float yPos;
	public float zPos;
	public float xAngle;
	public float yAngle;
	public float zAngle;
}
