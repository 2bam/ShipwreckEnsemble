using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ShuffleBag<T> {
	List<T> bag;
	int i;
	public ShuffleBag(IEnumerable<T> l) {
		bag = l.ToList();
		UnityEngine.Debug.Assert(bag.Count != 0);
		Reshuffle();
	}
	void Reshuffle() {
		bag = bag.OrderBy(x => UnityEngine.Random.value).ToList();
		i = 0;
	}
	public T Get() {
		var r = bag[i++];
		if(i == bag.Count)
			Reshuffle();
		return r;
	}
}
