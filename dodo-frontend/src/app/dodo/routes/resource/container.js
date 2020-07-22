import { connect } from "react-redux"
import { Resource } from "./resource"

import { resources, user } from "app/domain"
import { actions, selectors } from 'app/dodo/redux'

const { centerMap } = selectors
const { setCenterMap } = actions

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	resource: resources.selectors.currentResource(state),
	notifications: resources.selectors.currentNotifications(state),
	isLoading: resources.selectors.resourceLoading(state),
	resourceTypes: resources.selectors.resourceTypes(state),
	memberOf: user.selectors.memberOf(state),
	isLoggedIn: !!user.selectors.username(state),
	fetchingUser: user.selectors.fetchingUser(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getResource: (resourceType, resourceID, setCenterMap) => {
		resources.actions.resourceGet(dispatch, resourceType, resourceID, setCenterMap)
		resources.actions.notificationsGet(dispatch, resourceType, resourceID)
	},
	subscribeResource: (resourceType, resourceID, subscribe, body) =>
		resources.actions.subscribeResource(dispatch, resourceType, resourceID, subscribe, body),
})

export const ResourceConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Resource)
