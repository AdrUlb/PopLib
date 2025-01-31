using PopLib.Reanim;
using System.Text;
using System.Xml;

namespace PopLib.Particles;

public static class ParticlesXmlReader
{
	public static ParticlesEffect ReadFromStream(Stream stream)
	{
		using var streamReader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
		return ReadFromString(streamReader.ReadToEnd());
	}

	public static ParticlesEffect ReadFromString(string str)
	{
		using var stringReader = new StringReader("<root>" + str + "</root>");
		var settings = new XmlReaderSettings
		{
			IgnoreWhitespace = true
		};
		using var reader = XmlReader.Create(stringReader, settings);

		if (!reader.Read() || reader.NodeType != XmlNodeType.Element || reader.Name != "root")
			throw new("FIXME");

		var emitters = new List<ParticlesEmitter>();

		while (reader.Read())
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.EndElement:
					if (reader.Name != "root")
						throw new("FIXME");

					goto end;
				case XmlNodeType.Element:
					switch (reader.Name)
					{
						case "Emitter":
							emitters.Add(ReadEmitter(reader));
							break;
						default:
							throw new NotSupportedException($"Particles root tag {reader.Name} not supported.");
					}
					break;
				default:
					throw new InvalidDataException($"Encountered unexpected node of type {reader.NodeType} while parsing particles effect.");
			}
		}
		end:
		return new([..emitters]);
	}

	private static ParticlesEmitter ReadEmitter(XmlReader reader)
	{
		var emitter = new ParticlesEmitter();

		var fields = new List<ParticlesField>();

		while (reader.Read())
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.EndElement:
					if (reader.Name != "Emitter")
						throw new("FIXME");

					goto end;
				case XmlNodeType.Element:
					switch (reader.Name)
					{
						case "ImageFrames": emitter.ImageFrames = ReadIntParameter(reader, reader.Name); break;
						case "RandomLaunchSpin": emitter.RandomLaunchSpin = ReadBoolParameter(reader, reader.Name); break;
						case "SystemLoops": emitter.SystemLoops = ReadBoolParameter(reader, reader.Name); break;
						case "ParticleLoops": emitter.ParticleLoops = ReadBoolParameter(reader, reader.Name); break;
						case "RandomStartTime": emitter.RandomStartTime = ReadBoolParameter(reader, reader.Name); break;
						case "Additive": emitter.Additive = ReadBoolParameter(reader, reader.Name); break;
						case "HardwareOnly": emitter.HardwareOnly = ReadBoolParameter(reader, reader.Name); break;
						case "EmitterType": emitter.EmitterType = ReadEnumParameter<ParticlesEmitterType>(reader, reader.Name); break;
						case "Field": fields.Add(ReadField(reader)); break;
						case "Image": emitter.Image = ReadStringParameter(reader, reader.Name); break;
						case "Name": emitter.Name = ReadStringParameter(reader, reader.Name); break;
						case "SystemDuration": emitter.SystemDuration = ReadFloatParameterTrack(reader, reader.Name); break;
						case "SpawnRate": emitter.SpawnRate = ReadFloatParameterTrack(reader, reader.Name); break;
						case "SpawnMinActive": emitter.SpawnMinActive = ReadFloatParameterTrack(reader, reader.Name); break;
						case "SpawnMaxLaunched": emitter.SpawnMaxLaunched = ReadFloatParameterTrack(reader, reader.Name); break;
						case "EmitterRadius": emitter.EmitterRadius = ReadFloatParameterTrack(reader, reader.Name); break;
						case "EmitterOffsetX": emitter.EmitterOffsetX = ReadFloatParameterTrack(reader, reader.Name); break;
						case "EmitterOffsetY": emitter.EmitterOffsetY = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleDuration": emitter.ParticleDuration = ReadFloatParameterTrack(reader, reader.Name); break;
						case "LaunchSpeed": emitter.LaunchSpeed = ReadFloatParameterTrack(reader, reader.Name); break;
						case "LaunchAngle": emitter.LaunchAngle = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleRed": emitter.ParticleRed = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleGreen": emitter.ParticleGreen = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleBlue": emitter.ParticleBlue = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleAlpha": emitter.ParticleAlpha = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleBrightness": emitter.ParticleBrightness = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleSpinAngle": emitter.ParticleSpinAngle = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleSpinSpeed": emitter.ParticleSpinSpeed = ReadFloatParameterTrack(reader, reader.Name); break;
						case "ParticleScale": emitter.ParticleScale = ReadFloatParameterTrack(reader, reader.Name); break;
						default:
							throw new NotSupportedException($"Particles Emitter tag '{reader.Name}' not supported.");
					}
					break;
				default:
					throw new InvalidDataException($"Encountered unexpected node of type {reader.NodeType} while parsing particles Emitter.");
			}
		}
		end:

		if (fields.Count > 0)
			emitter.Fields = [..fields];

		return emitter;
	}

	private static string ReadStringParameter(XmlReader reader, string paramName)
	{
		if (!reader.Read() || reader.NodeType != XmlNodeType.Text)
			throw new("FIXME");

		var ret = reader.Value;

		if (!reader.Read() || reader.NodeType != XmlNodeType.EndElement || reader.Name != paramName)
			throw new($"FIXME");

		return ret;
	}

	private static int ReadIntParameter(XmlReader reader, string paramName) => int.Parse(ReadStringParameter(reader, paramName));

	private static T ReadEnumParameter<T>(XmlReader reader, string paramName) where T : struct
	{
		if (!Enum.TryParse<T>(ReadStringParameter(reader, paramName), out var value))
			throw new("FIXME");

		return value;
	}

	private static bool ReadBoolParameter(XmlReader reader, string paramName)
	{
		if (!reader.Read() || reader.NodeType != XmlNodeType.Text)
			throw new("FIXME");

		var ret = reader.Value switch
		{
			"0" => false,
			"1" => true,
			_ => throw new("FIXME")
		};

		if (!reader.Read() || reader.NodeType != XmlNodeType.EndElement || reader.Name != paramName)
			throw new($"FIXME");

		return ret;
	}

	private static ParticlesField ReadField(XmlReader reader)
	{
		var field = new ParticlesField();

		while (reader.Read())
		{
			switch (reader.NodeType)
			{
				case XmlNodeType.EndElement:
					if (reader.Name != "Field")
						throw new("FIXME");

					goto end;
				case XmlNodeType.Element:
					switch (reader.Name)
					{
						case "FieldType": field.FieldType = ReadEnumParameter<ParticlesFieldType>(reader, reader.Name); break;
						case "X": field.X = ReadFloatParameterTrack(reader, reader.Name); break;
						case "Y": field.Y = ReadFloatParameterTrack(reader, reader.Name); break;
						default:
							throw new NotSupportedException($"Particles Field tag '{reader.Name}' not supported.");
					}
					break;
				default:
					throw new InvalidDataException($"Encountered unexpected node of type {reader.NodeType} while parsing particles Emitter.");
			}
		}
		end:

		return field;
	}

	private static ParticlesFloatParameterTrack ReadFloatParameterTrack(XmlReader reader, string paramName) => ParseFloatParameterTrack(ReadStringParameter(reader, paramName));

	private static ParticlesFloatParameterTrack ParseFloatParameterTrack(string str)
	{
		str = str.Trim();

		if (str == "")
			return new([]);

		var nodes = new List<ParticlesFloatParameterTrackNode>();
		{
			var i = 0;

			// format := value [ ',' time ] [ ' ' CURVETYPE ]
			// value := NUMBER | ( '[' NUMBER ( ']' | ( NUMBER ']'  [ DISTRIBUTION ] ) )

			while (i < str.Length)
			{
				float lowValue;
				float highValue;
				var time = float.NaN;
				var curveType = ParticlesCurveType.Linear;
				var distribution = ParticlesCurveType.Linear;

				SkipWhitespace();

				if (str[i] == '[') // Distribution
				{
					i++;
					SkipWhitespace();
					lowValue = ReadFloat();
					SkipWhitespace();

					if (str[i] == ']')
					{
						i++;
						highValue = lowValue;
						distribution = ParticlesCurveType.Constant; // Not a distribution, constant.
					}
					else
					{
						highValue = ReadFloat();
						SkipWhitespace();

						if (str[i++] != ']')
							throw new("FIXME");

						var savedI = i;
						SkipWhitespace();

						if (i < str.Length && str[i] is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'))
						{
							if (!Enum.TryParse(ReadString(), out distribution))
								throw new("FIXME");
						}
						else
							i = savedI;
					}
				}
				else // Scalar
				{
					lowValue = highValue = ReadFloat();
				}

				if (i < str.Length && str[i] == ',')
				{
					i++;
					time = ReadFloat() / 100.0f;
				}
				
				SkipWhitespace();
				// Curve type
				if (str[i] is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'))
				{
					if (!Enum.TryParse(ReadString(), out curveType))
						throw new("FIXME");
				}

				var node = new ParticlesFloatParameterTrackNode(time, lowValue, highValue, curveType, distribution);
				nodes.Add(node);
			}

			void SkipWhitespace()
			{
				while (i < str.Length && char.IsWhiteSpace(str[i]))
					i++;
			}

			float ReadFloat()
			{
				var start = i;
				while (i < str.Length && str[i] is (>= '0' and <= '9') or '.' or '-')
					i++;

				return float.Parse(str[start..i]);
			}

			string ReadString()
			{
				var start = i;
				while (i < str.Length && str[i] is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'))
					i++;

				return str[start..i];
			}
		}
		if (nodes.Count == 0)
			throw new("FIXME");

		// If the first node does not have a defined time, set it to 0
		if (float.IsNaN(nodes[0].Time))
			nodes[0].Time = 0;

		// If the last node does not have a defined time, set it to 1
		if (nodes.Count > 1 && float.IsNaN(nodes[^1].Time))
			nodes[^1].Time = 1;

		// Interpolate between nodes
		var last = 0.0f;
		var delta = 0.0f;

		for (var i = 0; i < nodes.Count; i++)
		{
			// Figure out next delta
			if (!float.IsNaN(nodes[i].Time))
			{
				last = nodes[i].Time;

				if (i != nodes.Count - 1)
				{
					// Find next node with set value
					var j = i + 1;
					while (float.IsNaN(nodes[j].Time))
						j++;

					// Absolute difference between the next defined point in time and now
					var absoluteDelta = nodes[j].Time - nodes[i].Time;

					// The number of undefined steps
					delta = absoluteDelta / (j - i);
				}

				continue;
			}

			nodes[i].Time = last + delta;
		}

		return new([..nodes]);
	}
}
