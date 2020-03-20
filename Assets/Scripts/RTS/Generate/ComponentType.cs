using UnityEngine;

//自动生成请勿更改

namespace RTS
{
public partial class ComponentType : ComponentTypeBase
{
	public const int DBAnimComponent = 0;
	public const int RolePositionComponent = 1;
	public const int TeamComponent = 2;
	public const int MomentComponentBase = 3;
	public const int SkillComponent = 4;
	public const int MoveComponent = 5;
	public const int PlayerCommandBase = 6;
	public const int CommandComponent = 7;
	public const int SingletonComponent = 8;
	public const int MomentSingletonComponent = 9;
	public const int EntityRecordComponent = 10;
	public const int RecordComponent_T = 11;
	public const int SyncComponentBase = 12;
	public const int RoleStatusComponent = 13;
	public override int Count()
	{
		return 14;
	}



	public override int GetComponentIndex(string name) 
	{
		switch (name) 
		{

			 case "DBAnimComponent" : 
				 return DBAnimComponent ; 
			 case "RolePositionComponent" : 
				 return RolePositionComponent ; 
			 case "TeamComponent" : 
				 return TeamComponent ; 
			 case "MomentComponentBase" : 
				 return MomentComponentBase ; 
			 case "SkillComponent" : 
				 return SkillComponent ; 
			 case "MoveComponent" : 
				 return MoveComponent ; 
			 case "PlayerCommandBase" : 
				 return PlayerCommandBase ; 
			 case "CommandComponent" : 
				 return CommandComponent ; 
			 case "SingletonComponent" : 
				 return SingletonComponent ; 
			 case "MomentSingletonComponent" : 
				 return MomentSingletonComponent ; 
			 case "EntityRecordComponent" : 
				 return EntityRecordComponent ; 
			 case "RecordComponent_T" : 
				 return RecordComponent_T ; 
			 case "SyncComponentBase" : 
				 return SyncComponentBase ; 
			 case "RoleStatusComponent" : 
				 return RoleStatusComponent ; 
		}
		Debug.Log("未找到对应的组件 ：" + name); 
		return -1 ; 
	}
}
}
