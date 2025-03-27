<h1>Films API</h1>

<p>This is an API project I built using .NET API controllers. It allows you to do crud actions on films and limits you based on your JWT claims which much first be retrieved from the identity endpoint. "/token"</p>
<p>
  It's not intended to be a proper application that serves real users but rather, this was a way for me to put together everything I have learnt about API architecture. To that end, it is not hosted but can easily be cloned and inspected.
  Aswell as this, information that would normally be stored in a secure location such as the token signing key are just hardcoded for ease of use.
</p>

<h2>A list of concepts that I learnt about by building this application</h2>
<ul>
  <li>REST conventions</li>
  <li>JWT authentication and authorisation</li>
  <li>Fluent validation and validation middleware</li>
  <li>Static extensions for service registrations</li>
  <li>Request/Response contracts and static contract mapping</li>
</ul>
