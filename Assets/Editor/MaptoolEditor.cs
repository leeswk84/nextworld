using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MaptoolEditor : EditorWindow
{
	private string planet_width;
	private string planet_height;

	private string planet_all_type;

	private string planet_tile_all_h;
	private string planet_tile_all_random;

	private string planet_tile_h_w_start;
	private string planet_tile_h_w_end;
	private string planet_tile_h_h_start;
	private string planet_tile_h_h_end;
	private string planet_tile_h;
	private string planet_tile_h_random;

	private List<float> listBackup;
	private int backindex;

	private string str;

	[UnityEditor.MenuItem("HotKey/DeletePlayerPrefab", false, 1)]
	static public void DeletePlayerPrefab()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("PlayerPrefs.DeleteAll()");
	}

	[UnityEditor.MenuItem("Window/Open Maptool &`", false, 1)]
	static public void OpenExceltoCSVWindow()
	{
		MaptoolEditor[] windows = Resources.FindObjectsOfTypeAll<MaptoolEditor>();
		if (windows != null && windows.Length > 0)
		{
			EditorWindow.GetWindow<MaptoolEditor>(false, "Maptool", true).Close();
			return;
		}
		EditorWindow.GetWindow<MaptoolEditor>(false, "Maptool", true).Show();
	}

	private void OnGUI()
	{
		PrintGUI();
	}

	private void SetBackup()
	{
		if (listBackup == null) listBackup = new List<float>();
		listBackup.Clear();
		backindex = GameMgr.ins.mgrSave.editIndex;

		for (int i = 0; i < GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex].Count; i++)
		{
			 listBackup.Add(GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][i]);
		}
	}
	/// <summary>
	/// 진수 변환 10진수 n을 j진수로 변환하여 string으로 반환, j진수는 2보다 크거나 같아야 함
	/// </summary>
	private string ConvertJ(int n, int j, string str = "")
	{
		if (j < 2 || n < j) return n.ToString();
		int k = n / j;
		str = (n % j).ToString() + str;
		if (k >= j) return ConvertJ(k, j, str);
		else str = k.ToString() + str;
		return str;
	}
	/// <summary>
	/// 진수 변환 org진수 str을 10진수로 변환하여 int 로 반환
	/// </summary>
	private int Convert10(string str, int org)
	{
		int n = 0;
		int j;
		int add;

		for (int i = 0; i < str.Length; i++)
		{
			add = 1;
			for (j = 0; j < i; j++)	{ add *= org; }
			n += add * int.Parse(str.Substring(str.Length - i - 1, 1));
			//Debug.Log(add + "," + str.Substring(str.Length - i - 1, 1) + "," + n);
		}
		//Debug.Log(str.Substring(str.Length - 1, 1));
		//n += int.Parse(str.Substring(str.Length - 1, 1));

		return n;
	}

	public void PrintGUI()
	{
		if (GameMgr.ins == null)
		{
			GUILayout.Label("PLAY 중 사용 가능 합니다.");
			
			if (GUILayout.Button("문제풀이"))
			{

				#region
				/*
				string s = "baby";
				long answer = 0;
				System.Text.StringBuilder str;
				int add = 0;

				for (int i = 0; i < s.Length + 1; i++)
				{
					str = new System.Text.StringBuilder();
					//str.Append(s.Substring(i, 1));
					for (int j = i + 1; j < s.Length + 1; j++)
					{
						str.Append(s.Substring(j-1, 1));
						//str = new System.Text.StringBuilder();
						//str. = s.Substring(i, j - i);
						add = 0;
						//Debug.Log(str);
						
						for (int z = 0; z < str.Length; z++)
						{
							for (int x = z + 1; x < str.Length; x++)
							{
								//if (s.Substring(z, 1) == s.Substring(x, 1)) continue;
								if (s.Substring(i + z, 1) == s.Substring(i + x, 1)) continue;

								if (add < x - z) add = x - z;
							}
						}
						answer += add;
						
					}
				}
				Debug.Log(answer);
				*/
				/*
				System.Collections.Generic.Dictionary<int, bool> dic = new System.Collections.Generic.Dictionary<int, bool>();
				System.Collections.Generic.List<int> list = new System.Collections.Generic.List<int>();
				int[] numbers = new int[] {2,1,3,4,1};
				int i, j;
				for (i = 0; i < numbers.Length; i++)
				{
					for (j = i + 1; j < numbers.Length; j++)
					{
						if (dic.ContainsKey(numbers[i] + numbers[j])) continue;
						dic.Add(numbers[i] + numbers[j], true);
					}
				}

				foreach (int key in dic.Keys)
				{
					list.Add(key);
				}
				list.Sort();
				int[] answer = list.ToArray();

				string log = string.Empty;
				for (i = 0; i < answer.Length; i++) {	log += answer[i] + ",";	}
				Debug.Log(log);
				*/
				/*
				int[] answers = new int[] { 1, 2, 3, 4, 5 };
				//int[] answers = new int[] { 1, 3, 2, 4, 2 };
				int[] student1 = new int[] { 1, 2, 3, 4, 5 };
				int[] student2 = new int[] { 2, 1, 2, 3, 2, 4, 2, 5 };
				int[] student3 = new int[] { 3, 3, 1, 1, 2, 2, 4, 4, 5, 5 };

				int score1 = 0;
				int score2 = 0;
				int score3 = 0;

				int i;
				for (i = 0; i < answers.Length; i++)
				{
					if (answers[i] == student1[i % student1.Length]) score1++;
					if (answers[i] == student2[i % student2.Length]) score2++;
					if (answers[i] == student3[i % student3.Length]) score3++;
				}
				List<int> score = new List<int>();
				score.Add(score1);
				score.Add(score2);
				score.Add(score3);
				score.Sort();
				score.Reverse();

				for (i = 0; i < score.Count; i++)
				{
					if (i + 1 >= score.Count) break;
					if (score[i] > score[i + 1])
					{
						score.RemoveAt(i+1);
						i--;
					}
				}
				List<int> result = new List<int>();

				for (i = 0; i < score.Count; i++)
				{
					if (score[i] == score1 && result.Contains(1) == false) result.Add(1);
					if (score[i] == score2 && result.Contains(2) == false) result.Add(2);
					if (score[i] == score3 && result.Contains(3) == false) result.Add(3);
				}
				result.Sort();

				int[] answer = result.ToArray();
				string log = string.Empty;
				for (i = 0; i < answer.Length; i++) { log += answer[i] + ","; }
				Debug.Log(log);
				*/

				/*
				int n = 5;
				int[] lost = new int[] { 2, 4};
				int[] reserve = new int[] { 3};

				int i;
				HashSet<int> hashL = new HashSet<int>();
				HashSet<int> hashR = new HashSet<int>();

				for (i = 0; i < lost.Length; i++) { hashL.Add(lost[i]);	 }
				for (i = 0; i < reserve.Length; i++)
				{
					if (hashL.Contains(reserve[i]))
					{
						hashL.Remove(reserve[i]);
						continue;
					}
					hashR.Add(reserve[i]);
				}

				foreach (int v in hashR)
				{
					if (hashL.Contains(v - 1))
					{
						hashL.Remove(v-1);
						continue;
					}
					if (hashL.Contains(v + 1))
					{
						hashL.Remove(v + 1);
						continue;
					}
				}

				int result = n - hashL.Count;
				Debug.Log(result);
				*/
				/*
				int[] array = new int[] { 1, 5, 2, 6, 3, 7, 4 };
				int[,] commands = new int[,]{ { 2, 5, 3 }, { 4, 4, 1 }, { 1, 7, 3 }, { 1, 7, 3 } };

				int[] answer = new int[commands.GetLength(0)];
				List<int> list = new List<int>();

				int i, j;
				for (i = 0; i < commands.GetLength(0); i++)
				{
					list.Clear();
					for (j = commands[i, 0] - 1; j < commands[i, 1]; j++)
					{
						list.Add(array[j]);
					}
					list.Sort();
					answer[i] = list[commands[i, 2] - 1];

					//Debug.Log(answer[i]);
				}
				*/
				/*
				int a = 5;
				int b = 24;
				
				System.DateTime date = new System.DateTime(2016, a, b);
				string answer = "MON";
				switch (date.DayOfWeek)
				{
					case System.DayOfWeek.Monday: answer = "MON"; break;
					case System.DayOfWeek.Tuesday: answer = "TUE"; break;
					case System.DayOfWeek.Wednesday: answer = "WED"; break;
					case System.DayOfWeek.Thursday: answer = "THU"; break;
					case System.DayOfWeek.Friday: answer = "FRI"; break;
					case System.DayOfWeek.Saturday: answer = "SAT";  break;
					case System.DayOfWeek.Sunday: answer = "SUN"; break;
				}
				Debug.Log(answer);
				*/
				#endregion

				int n = 1;// 45;
				
				string str = ConvertJ(n, 3);
				Debug.Log(str);
				List<string> listStr = new List<string>();
				int i;
				for (i = 0; i < str.Length; i++)
				{	listStr.Add(str.Substring(i, 1));	}
				listStr.Reverse();
				str = string.Empty;
				for (i = 0; i < listStr.Count; i++)
				{ str += listStr[i]; }
				Debug.Log(str);
				
				n = Convert10(str, 3);
				
				int answer = 0;
				answer = n;

				Debug.Log(answer);
				//using System.Collections.Generic;
			}

			return;
		}
		
		GUILayout.Label("행성 정보");
		GUILayout.Label("INDEX : " + GameMgr.ins.mgrSave.PLANET_IDX);
		GUILayout.Label("block 가로갯수 : " + SaveManager.PLANET_WIDTH + ", block 세로갯수:" + SaveManager.PLANET_HEIGHT);
		GUILayout.Label("표지중인 block index: " + GameMgr.ins.mgrSave.editIndex);
		str = "중심에 표지중인 block index: ";
		if (GameMgr.ins.mgrBlock.posBlocks != null && GameMgr.ins.mgrBlock.posBlocks.Length > 0) str += GameMgr.ins.mgrBlock.posBlocks[12].block.name;
		GUILayout.Label(str);

		if (GUILayout.Button("되돌리기"))
		{
			if (listBackup == null) return;
			if(backindex != GameMgr.ins.mgrSave.editIndex)
			{
				listBackup = null;
				return;
			}

			for (int i = 0; i < GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex].Count; i++)
			{
				GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][i] = listBackup[i];
			}
			GameMgr.ins.mgrBlock.posBlocks[12].mgrGround.SetGround(GameMgr.ins.mgrSave.editIndex);
		}

		GUILayout.Label("");
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("행성 block 갯수 설정");
			if (GUILayout.Button("초기화"))
			{
				planet_width = "";
				planet_height = "";
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("가로갯수(0)");
			planet_width = GUILayout.TextArea(planet_width);
			GUILayout.Label("세로갯수(0)");
			planet_height = GUILayout.TextArea(planet_height);

			if (GUILayout.Button("설정"))
			{
				int pw = 0;
				if(int.TryParse(planet_width, out pw) == false) pw = 0;
				int ph = 0;
				if (int.TryParse(planet_height, out ph) == false) ph = 0;

				//Debug.Log(planet_width + "," + planet_height);
				GameMgr.ins.mgrNetwork.SendMapCountEdit(pw, ph);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("행성 모든 block type 설정");
			if (GUILayout.Button("초기화"))
			{
				planet_all_type = "";
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("type(0)");
			planet_all_type = GUILayout.TextArea(planet_all_type);
			if (GUILayout.Button("설정"))
			{
				int type = 0;
				if (int.TryParse(planet_all_type, out type) == false) type = 0;

				string maps = string.Empty;
				int pos;
				int mtype = int.Parse(planet_all_type);
				for (int i = 0; i < SaveManager.PLANET_WIDTH * SaveManager.PLANET_HEIGHT; i++)
				{
					if (i != 0) maps += ",";
					maps += i;
					if (GameMgr.ins.mgrBlock.GetBlockIdx(i) == null) continue;
					pos = GameMgr.ins.mgrBlock.GetBlockIdx(i).pos_idx;
					GameMgr.ins.mgrSave.blockTypes[i] = mtype;
					GameMgr.ins.mgrBlock.posBlocks[pos].mgrGround.SetGround(i);
				}
				GameMgr.ins.mgrSide.is_edit = false;
				GameMgr.ins.mgrSide.SetSideUV(true);
				GameMgr.ins.mgrSide.SetGroundSideColor(false);
				GameMgr.ins.mgrSide.is_edit = true;
				
				//Debug.Log(maps + "," + planet_type);
				GameMgr.ins.mgrNetwork.SendMapEdittype(maps, mtype);
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("행성 현제 block 모든 tile 높이 설정 (우측상단에서 부터 INDEX)");
			if (GUILayout.Button("초기화"))
			{
				planet_tile_all_h = "";
				planet_tile_all_random = "";
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("높이(0)");
			planet_tile_all_h = GUILayout.TextArea(planet_tile_all_h);
			GUILayout.Label("임의값 범위(0)");
			planet_tile_all_random = GUILayout.TextArea(planet_tile_all_random);

			if (GUILayout.Button("설정"))
			{
				SetBackup();

				float setY = 0f;
				if (float.TryParse(planet_tile_all_h, out setY) == false) setY = 0f;

				float random = 0f;
				if (float.TryParse(planet_tile_all_random, out random) == false) random = 0f;

				float addY = 0f;
				//string tiley = string.Empty;
				for (int i = 0; i < GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex].Count; i++)
				{
					addY = Random.Range(0f, random);
					GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][i] = setY + addY;
					//if (i != 0) tiley += ",";
					//tiley += GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][i];
				}
				//GameMgr.ins.mgrBlock.posBlocks[0].SetPosIdx(GameMgr.ins.mgrSave.editIndex);
				//GameMgr.ins.mgrSave.SetTiles();
				//Debug.Log(tiley);
				GameMgr.ins.mgrBlock.posBlocks[12].mgrGround.SetGround(GameMgr.ins.mgrSave.editIndex);
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("행성 현제 block 가로세로 열 tile 높이 설정");
			if (GUILayout.Button("초기화"))
			{
				planet_tile_h_w_start = "";
				planet_tile_h_w_end = "";
				planet_tile_h_h_start = "";
				planet_tile_h_h_end = "";
				planet_tile_h = "";
				planet_tile_h_random = "";
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("가로 시작(0)");
			planet_tile_h_w_start = GUILayout.TextArea(planet_tile_h_w_start);
			GUILayout.Label("가로 끝("+ (GameMgr.WIDTH + 1) + ")");
			planet_tile_h_w_end = GUILayout.TextArea(planet_tile_h_w_end);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("세로 시작(0)");
			planet_tile_h_h_start = GUILayout.TextArea(planet_tile_h_h_start);
			GUILayout.Label("세로 끝("+(GameMgr.WIDTH + 1)+")");
			planet_tile_h_h_end = GUILayout.TextArea(planet_tile_h_h_end);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label("높이(0)", GUILayout.ExpandWidth(true));
			planet_tile_h = GUILayout.TextArea(planet_tile_h);
			GUILayout.Label("임의 범위(0)");
			planet_tile_h_random = GUILayout.TextArea(planet_tile_h_random);
			
			if (GUILayout.Button("설정"))
			{
				SetBackup();

				float setY = 0f;
				float.TryParse(planet_tile_h, out setY);

				float random = 0f;
				float.TryParse(planet_tile_h_random, out random);
				float addY = 0f;
				int startw = 0;
				int.TryParse(planet_tile_h_w_start, out startw);
				int endw = 0;
				if(int.TryParse(planet_tile_h_w_end, out endw) == false) endw = GameMgr.WIDTH + 1;

				int starth = 0;
				int.TryParse(planet_tile_h_h_start, out starth);
				int endh = 0;
				if(int.TryParse(planet_tile_h_h_end, out endh) == false) endh = GameMgr.WIDTH + 1;

				//string tiley = string.Empty;
				int i, j, idx;
				for (i = startw; i < endw; i++)// GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex].Count; i++)
				{
					for (j = starth; j < endh; j++)
					{
						idx = j + ((GameMgr.WIDTH + 1) * i);
						addY = Random.Range(0f, random);
						GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][idx] = setY + addY;
					}
					//if (i != 0) tiley += ",";
					//tiley += GameMgr.ins.mgrSave.dicGround[GameMgr.ins.mgrSave.editIndex][i];
				}
				//GameMgr.ins.mgrBlock.posBlocks[0].SetPosIdx(GameMgr.ins.mgrSave.editIndex);
				//GameMgr.ins.mgrSave.SetTiles();
				//Debug.Log(tiley);
				GameMgr.ins.mgrBlock.posBlocks[12].mgrGround.SetGround(GameMgr.ins.mgrSave.editIndex);


				
			}
		}
		GUILayout.EndHorizontal();

	}
}
