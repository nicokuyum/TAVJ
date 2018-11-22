
using System;
using System.Collections.Generic;

public static class AI
{
	static Dictionary<int, int> duration = new Dictionary<int, int>();
	static Dictionary<int, Random> rands = new Dictionary<int, Random>();
	static Dictionary<int, PlayerAction> actions= new Dictionary<int, PlayerAction>();
	private static Random SeedGenerator = new Random(9845105);
	
	public static void add(int id)
	{
		rands.Add(id, new Random(SeedGenerator.Next(int.MaxValue)));
		duration.Add(id, 0);
		actions.Add(id, PlayerAction.MoveForward);
	}
	
	public static void act(PlayerSnapshot ps)
	{
		if (duration[ps.id] == 0)
		{
			double action = rands[ps.id].NextDouble();
			PlayerAction act = PlayerAction.MoveBack;
			if (action < 0.25)
			{
				act = PlayerAction.MoveForward;
			}else if (action < 0.5)
			{
				act = PlayerAction.MoveRight;

			}else if (action < 0.75)
			{
				act = PlayerAction.MoveBack;

			}else if (action < 1)
			{
				act = PlayerAction.MoveLeft;
			}
			actions[ps.id] = act;
			duration[ps.id] = GlobalSettings.AIDuration;
		}
		
		Mover.GetInstance().ApplyAction(ps, actions[ps.id],1.0f/GlobalSettings.Fps);
		duration[ps.id]--;
	}
	
}
