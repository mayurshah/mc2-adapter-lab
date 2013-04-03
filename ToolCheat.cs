MTConnect.CuttingTool.LifeType type = MTConnect.CuttingTool.LifeType.MINUTES;
 if (lifeType.Text == "PART_COUNT")
     type = MTConnect.CuttingTool.LifeType.PART_COUNT;
 else if (lifeType.Text == "WEAR")
     type = MTConnect.CuttingTool.LifeType.WEAR;

 MTConnect.CuttingTool.Direction dir = MTConnect.CuttingTool.Direction.UP;
 if (lifeDirection.Text == "DOWN")
     dir = MTConnect.CuttingTool.Direction.DOWN;

 tool.AddLife(type, dir, lifeValue.Text, lifeInitial.Text, lifeLimit.Text);

 tool.AddProperty("ProcessSpindleSpeed", new string[] 
     { "nominal", speedNominal.Text,
       "minimum", speedMin.Text,
       "maximum", speedMax.Text}, speed.Text);

 tool.AddMeasurement("FunctionalLength", "LF", Double.Parse(lengthVal.Text), Double.Parse(lengthNom.Text), 
     Double.Parse(lengthMin.Text), Double.Parse(lengthMax.Text));
 tool.AddMeasurement("CuttingDiameterMax", "DC", Double.Parse(diaVal.Text), Double.Parse(diaNom.Text),
     Double.Parse(diaMin.Text), Double.Parse(diaMax.Text));