using System;

using DOL.GS;

namespace ServerPlayer
{
	public class FollowPlayerAction : RegionAction
	{
		private GamePlayer follower;
		private GamePlayer followed;
		
		public FollowPlayerAction (GamePlayer follower, GamePlayer followed)
			: base(follower)
		{
			this.followed = followed;
			this.follower = follower;
		}
		
		protected override void OnTick()
		{
			follower.LastPositionUpdateTick = Environment.TickCount;
			follower.LastPositionUpdatePoint.X = follower.X;
			follower.LastPositionUpdatePoint.Y = follower.Y;
			follower.LastPositionUpdatePoint.Z = follower.Z;
			
			follower.SetGroundTarget(followed.X, followed.Y, followed.Z);
			
			follower.MoveTo(
				followed.CurrentRegion.ID,
				followed.X,
				followed.Y,
				followed.Z,
				followed.Heading);
		}
	}
}

