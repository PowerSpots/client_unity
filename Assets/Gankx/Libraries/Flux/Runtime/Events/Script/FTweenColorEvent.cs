namespace Flux
{
    [FEvent("Script/Tween Color")]
	public class FTweenColorEvent : FTweenVariableEvent<FTweenColor>  {

		protected override object GetValueAt( float t )
		{
			return _tween.GetValue( t );
		}
	}
}
