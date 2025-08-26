import 'package:flutter/material.dart';

void main() {
  runApp(ConversionApp());
}

class ConversionApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Unit Converter',
      theme: ThemeData(primarySwatch: Colors.blue),
      home: ConverterScreen(),
    );
  }
}

class ConverterScreen extends StatefulWidget {
  @override
  _ConverterScreenState createState() => _ConverterScreenState();
}

class _ConverterScreenState extends State<ConverterScreen> {
  final TextEditingController _controller = TextEditingController();
  String _category = 'Distance';
  String _fromUnit = 'Miles';
  String _toUnit = 'Kilometers';
  String _result = '';

  final Map<String, List<String>> units = {
    'Distance': ['Miles', 'Kilometers'],
    'Weight': ['Kilograms', 'Pounds'],
  };

  void _convert() {
    double? value = double.tryParse(_controller.text);
    if (value == null) {
      setState(() {
        _result = 'Please enter a valid number.';
      });
      return;
    }
    double converted = 0;
    if (_category == 'Distance') {
      if (_fromUnit == 'Miles' && _toUnit == 'Kilometers') {
        converted = value * 1.60934;
      } else if (_fromUnit == 'Kilometers' && _toUnit == 'Miles') {
        converted = value / 1.60934;
      } else {
        converted = value;
      }
    } else if (_category == 'Weight') {
      if (_fromUnit == 'Kilograms' && _toUnit == 'Pounds') {
        converted = value * 2.20462;
      } else if (_fromUnit == 'Pounds' && _toUnit == 'Kilograms') {
        converted = value / 2.20462;
      } else {
        converted = value;
      }
    }
    setState(() {
      _result = '$value $_fromUnit = ${converted.toStringAsFixed(2)} $_toUnit';
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Unit Converter')),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            DropdownButton<String>(
              value: _category,
              items: units.keys
                  .map((cat) => DropdownMenuItem(
                        value: cat,
                        child: Text(cat),
                      ))
                  .toList(),
              onChanged: (val) {
                setState(() {
                  _category = val!;
                  _fromUnit = units[_category]![0];
                  _toUnit = units[_category]![1];
                });
              },
            ),
            SizedBox(height: 16),
            Row(
              children: [
                Expanded(
                  child: DropdownButton<String>(
                    value: _fromUnit,
                    items: units[_category]!
                        .map((unit) => DropdownMenuItem(
                              value: unit,
                              child: Text(unit),
                            ))
                        .toList(),
                    onChanged: (val) {
                      setState(() {
                        _fromUnit = val!;
                      });
                    },
                  ),
                ),
                SizedBox(width: 16),
                Icon(Icons.arrow_forward),
                SizedBox(width: 16),
                Expanded(
                  child: DropdownButton<String>(
                    value: _toUnit,
                    items: units[_category]!
                        .map((unit) => DropdownMenuItem(
                              value: unit,
                              child: Text(unit),
                            ))
                        .toList(),
                    onChanged: (val) {
                      setState(() {
                        _toUnit = val!;
                      });
                    },
                  ),
                ),
              ],
            ),
            SizedBox(height: 16),
            TextField(
              controller: _controller,
              keyboardType: TextInputType.numberWithOptions(decimal: true),
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                labelText: 'Enter value',
              ),
            ),
            SizedBox(height: 16),
            ElevatedButton(
              onPressed: _convert,
              child: Text('Convert'),
            ),
            SizedBox(height: 24),
            Text(
              _result,
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }
}
