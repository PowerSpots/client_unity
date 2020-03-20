namespace Flux
{
    [FEvent("Script/Tween Vector3")]
	public class FTweenVector3Event : FTweenVariableEvent<FTweenVector3>  {

		protected override object GetValueAt( float t )
		{
			return _tween.GetValue( t );
		}
	}
}
