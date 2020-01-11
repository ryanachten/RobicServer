import ApolloClient from "apollo-client";
import React from "react";
import { ApolloProvider } from "react-apollo";
import ReactDOM from "react-dom";

const client = new ApolloClient({});

const Root = () => {
  window.location.href = "http://robic.herokuapp.com";
  return (
    <ApolloProvider client={client}>
      <div />
    </ApolloProvider>
  );
};

ReactDOM.render(<Root />, document.querySelector("#root"));
