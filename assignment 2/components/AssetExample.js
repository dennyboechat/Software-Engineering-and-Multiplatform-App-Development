import { Text, View, StyleSheet, Image } from 'react-native';
import profilePic from '../assets/profile.jpg';

export default function AssetExample() {
  return (
    <View style={styles.container}>
      <Text style={styles.paragraph}>
        WELCOME TO THE UNIVERSITY of the CUMBERLANDS{"\n"}
        Course ID: MSCS 533{"\n\n"}
      </Text>
      <View style={styles.container2}>
        <Text style={styles.paragraph2}>
        {/* The code below will display my BioSketch @ UC, and it's amazing!!!!! */}
        My name is Denny Boechat. I am a passionate software engineer with 10+ years of experience in mobile and web development. I love building innovative solutions and mentoring students at UC. My research interests include AI, cloud computing, and mobile UX.
        </Text>
        <Image source={profilePic} style={styles.image} />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
    backgroundColor: '#e60026',
    flex: 1,
  },
  container2: {
    alignItems: 'center',
    padding: 5,
    backgroundColor: 'white',
  },
  paragraph: {
    margin: 24,
    fontSize: 18,
    fontWeight: 'bold',
    textAlign: 'center',
    color: 'black',
  },
  paragraph2: {
    margin: 24,
    fontSize: 18,
    textAlign: 'center',
    color: 'black',
  },
  image: {
    width: 120,
    height: 120,
    marginBottom: '10px'
  },
});