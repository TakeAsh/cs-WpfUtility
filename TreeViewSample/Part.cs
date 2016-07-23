using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TakeAshUtility;

namespace TreeViewSample {

    public class Part {

        public string Name { get; set; }

        public List<Part> Children { get; set; }

        public virtual string NodeLabel {
            get {
                if (Children != null) {
                    var counter = Children.Aggregate(
                        new Dictionary<string, int>(),
                        (current, part) => {
                            var name = part.GetType().Name;
                            if (current.ContainsKey(name)) {
                                ++current[name];
                            } else {
                                current[name] = 1;
                            }
                            return current;
                        }
                    );
                    return Name + "\n" +
                        counter.Select(kv => kv.Key + ":" + kv.Value).JoinToString(", ");
                } else {
                    return Name;
                }
            }
        }
    }

    public class Body : Part {

        public Body() {
            Name = "Body";
            Children = new List<Part>() {
                new Head() {
                    Name = "Head",
                    Children = new List<Part>() {
                        new Eye() {
                            Name = "Left Eye",
                        },
                        new Eye() {
                            Name = "Right Eye",
                        },
                        new Mouse() {
                            Name = "Mouse",
                        },
                    },
                },
                new Arm() {
                    Name = "Left Arm",
                    Children = new List<Part>() {
                        new Hand() {
                            Name = "Left Hand",
                        },
                    },
                },
                new Arm() {
                    Name = "Right Arm",
                    Children = new List<Part>() {
                        new Hand() {
                            Name = "Right Hand",
                        },
                    },
                },
                new Leg() {
                    Name = "Left Leg",
                    Children = new List<Part>() {
                        new Foot() {
                            Name = "Left Foot",
                        },
                    },
                },
                new Leg() {
                    Name = "Right Leg",
                    Children = new List<Part>() {
                        new Foot() {
                            Name = "Right Foot",
                        },
                    },
                },
            };
        }
    }

    public class Head : Part { }

    public class Eye : Part { }

    public class Mouse : Part { }

    public class Arm : Part { }

    public class Hand : Part { }

    public class Leg : Part { }

    public class Foot : Part { }
}
