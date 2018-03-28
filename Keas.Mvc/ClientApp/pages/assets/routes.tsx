import * as React from "react";
import { Route } from "react-router-dom";

import EquipmentContainer from "../../components/Equipment/EquipmentContainer";
import KeyContainer from "../../components/Keys/KeyContainer";
import PeopleContainer from "../../components/People/PeopleContainer";
import PersonContainer from "../../components/PersonContainer";
import AssetNav from "../assets/AssetNav";
import AssetContainer from "./AssetContainer";

// examples of common routes
// /CAESDO/keys/display/12
// /CAESDO/people/
// /CAESDO/person/1/keys/details/12
export const routes = (
  <AssetNav>
    <Route path="/:team/keys/:action?/:id?" component={KeyContainer} />
    <Route
      path="/:team/equipment/:action?/:id?"
      component={EquipmentContainer}
    />
    <Route path="/:team/people/:action?/:id?" component={PeopleContainer} />
    <Route
      path="/:team/person/:id?/:assetType?/:action?/:subId?"
      component={PersonContainer}
    />
  </AssetNav>
);
