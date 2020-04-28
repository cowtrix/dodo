import React from "react"
import { Link } from "react-router-dom"

import { PageTitle } from "app/components/page-title"
import { DateLayout } from "app/components/date-layout"
import { Button } from "app/components/button"
import styles from "./rebellion-detail.module.scss"

export const RebellionDetail = ({ rebellion }) => {
	return (
		<DateLayout
			startDate={
				rebellion.startDate ? new Date(rebellion.startDate) : null
			}
			endDate={rebellion.endDate ? new Date(rebellion.endDate) : null}
			title={<PageTitle title={rebellion.name} subTitle="Glasgow" />}
		>
			<div className={styles.detail}>{rebellion.publicDescription}</div>
			<div className={styles.join}>
				<Button
					as={<Link to={"/rebellion/join"} />}
					style={{ display: "inline-block" }}
				>
					JOIN REBELLION
				</Button>
			</div>
		</DateLayout>
	)
}
