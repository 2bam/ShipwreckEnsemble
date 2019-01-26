using UnityEngine;
using System.Collections.Generic;

public interface INode {
	IEnumerable<INode> neighbors { get; }
	Vector2 position { get; }
}
