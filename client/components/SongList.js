import gql from "graphql-tag";
import React from "react";
import { graphql } from "react-apollo";

class SongList extends React.Component {
  renderSongs() {
    const songs = this.props.data.songs;
    if (!songs) return null;
    return songs.map(song => <li key={song.id}>{song.title}</li>);
  }

  render() {
    const loading = this.props.data.loading;
    if (loading) return <div>Loading...</div>;
    return <ul>{this.renderSongs()}</ul>;
  }
}

const query = gql`
  {
    songs {
      id
      title
    }
  }
`;

export default graphql(query)(SongList);
