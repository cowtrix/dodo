import React from "react"
import PropTypes from "prop-types"
import { Button, Selector as SelectorWrapper } from "app/components"
import { Selector } from "./selector"

const title = "Within..."

export const Date = ({ withinStartDate, withinEndDate, updateDate }) => (
	<SelectorWrapper
		title={title}
		content={
			<Selector
				placeholder={title}
				withinStartDate={withinStartDate}
				withinEndDate={withinEndDate}
				updateDate={updateDate}
			/>
		}
	/>
)

Date.propTypes = {
	withinStartDate: PropTypes.number,
	withinEndDate: PropTypes.number,
	updateDate: PropTypes.func
}
