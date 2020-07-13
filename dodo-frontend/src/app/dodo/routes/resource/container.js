import { connect } from "react-redux"
import { Resource } from "./resource"

import { resources } from "app/domain"
import { actions, selectors } from 'app/dodo/redux'

const { centerMap } = selectors
const { setCenterMap } = actions

const mapStateToProps = state => ({
	centerMap: centerMap(state),
	resource: resources.selectors.currentResource(state),
	notifications: resources.selectors.currentNotifications(state),
	isLoading: resources.selectors.resourceLoading(state),
	resourceTypes: resources.selectors.resourceTypes(state)
})

const mapDispatchToProps = dispatch => ({
	setCenterMap: (centerMap) => dispatch(setCenterMap(centerMap)),
	getResource: (resourceType, resourceID, setCenterMap) =>
		resources.actions.resourceGet(dispatch, resourceType, resourceID, setCenterMap),
	getNotifications: (resourceType, resourceID) => resources.actions.notificationsGet(dispatch, resourceType, resourceID)
})

export const ResourceConnected = connect(
	mapStateToProps,
	mapDispatchToProps
)(Resource)
